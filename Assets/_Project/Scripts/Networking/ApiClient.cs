using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PetAdventure.Pets;
using UnityEngine;
using UnityEngine.Networking;

namespace PetAdventure.Networking
{
    [Serializable]
    public sealed class AccountResponseDto
    {
        public int id;
        public string username;
    }

    [Serializable]
    public sealed class LoginResponseDto
    {
        public string token;
        public string username;
    }

    [Serializable]
    public sealed class CharacterResponseDto
    {
        public int id;
        public string name;
        public string @class;
        public int level;
        public int hp;
        public int accountId;

        public PetRuntimeData ToRuntime()
        {
            return new PetRuntimeData
            {
                id = id,
                name = name,
                element = PetElementInfo.Parse(@class),
                level = level,
                hp = hp,
                accountId = accountId,
                petKey = (@class ?? "").ToLowerInvariant()
            };
        }
    }

    [Serializable]
    public sealed class CharacterListDto
    {
        public CharacterResponseDto[] items;
    }

    public readonly struct ApiResult<T>
    {
        public ApiResult(bool success, int status, T value, string error)
        {
            Success = success;
            Status = status;
            Value = value;
            Error = error;
        }

        public bool Success { get; }

        public int Status { get; }

        public T Value { get; }

        public string Error { get; }
    }

    /// <summary>Gọi 4 API ASM 1 + GET danh sách. Không dùng Newtonsoft, bọc mảng bằng {"items":...}.</summary>
    public sealed class ApiClient
    {
        private readonly string baseUrl;

        public string Token { get; set; } = "";

        public ApiClient(string baseUrl)
        {
            this.baseUrl = baseUrl.TrimEnd('/');
        }

        public async Task<ApiResult<AccountResponseDto>> RegisterAsync(string username, string password,
            CancellationToken cancellationToken = default)
        {
            string body = JsonUtility.ToJson(new RegisterPayload { username = username, password = password });
            using UnityWebRequest request = Post(baseUrl + "/api/auth/register", body, null);
            return await SendAsync<AccountResponseDto>(request, cancellationToken);
        }

        public async Task<ApiResult<LoginResponseDto>> LoginAsync(string username, string password,
            CancellationToken cancellationToken = default)
        {
            string body = JsonUtility.ToJson(new LoginPayload { username = username, password = password });
            using UnityWebRequest request = Post(baseUrl + "/api/auth/login", body, null);
            ApiResult<LoginResponseDto> result = await SendAsync<LoginResponseDto>(request, cancellationToken);
            if (result.Success && result.Value != null)
            {
                Token = result.Value.token;
            }

            return result;
        }

        public async Task<ApiResult<CharacterResponseDto[]>> GetCharactersAsync(
            CancellationToken cancellationToken = default)
        {
            using UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/api/characters");
            SetAuth(request);
            ApiResult<string> raw = await SendRawAsync(request, cancellationToken);
            if (!raw.Success) return new ApiResult<CharacterResponseDto[]>(false, raw.Status, null, raw.Error);

            try
            {
                CharacterListDto wrapped = JsonUtility.FromJson<CharacterListDto>(
                    "{\"items\":" + raw.Value + "}");
                CharacterResponseDto[] items = wrapped != null && wrapped.items != null
                    ? wrapped.items
                    : Array.Empty<CharacterResponseDto>();
                return new ApiResult<CharacterResponseDto[]>(true, raw.Status, items, "");
            }
            catch (Exception exception)
            {
                return new ApiResult<CharacterResponseDto[]>(false, raw.Status, null, "JSON lỗi: " + exception.Message);
            }
        }

        public async Task<ApiResult<CharacterResponseDto>> CreateCharacterAsync(string name, string element,
            CancellationToken cancellationToken = default)
        {
            string body = JsonUtility.ToJson(new CreatePayload { name = name, @class = element });
            using UnityWebRequest request = Post(baseUrl + "/api/characters", body, Token);
            return await SendAsync<CharacterResponseDto>(request, cancellationToken);
        }

        public async Task<ApiResult<CharacterResponseDto>> UpdateCharacterAsync(int id, string name, string element,
            int level, int hp, CancellationToken cancellationToken = default)
        {
            string body = JsonUtility.ToJson(new UpdatePayload
            {
                name = name,
                @class = element,
                level = level,
                hp = hp
            });
            using UnityWebRequest request = Put(baseUrl + "/api/characters/" + id, body, Token);
            return await SendAsync<CharacterResponseDto>(request, cancellationToken);
        }

        private UnityWebRequest Post(string url, string body, string token)
        {
            UnityWebRequest request = UnityWebRequest.Post(url, body, "application/json");
            SetAuth(request);
            return request;
        }

        private UnityWebRequest Put(string url, string body, string token)
        {
            UnityWebRequest request = new UnityWebRequest(url, "PUT")
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body)),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");
            SetAuth(request);
            return request;
        }

        private void SetAuth(UnityWebRequest request)
        {
            if (!string.IsNullOrEmpty(Token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + Token);
            }
        }

        private static async Task<ApiResult<T>> SendAsync<T>(UnityWebRequest request, CancellationToken cancellationToken)
            where T : class
        {
            ApiResult<string> raw = await SendRawAsync(request, cancellationToken);
            if (!raw.Success) return new ApiResult<T>(false, raw.Status, null, raw.Error);

            try
            {
                T value = JsonUtility.FromJson<T>(raw.Value);
                return new ApiResult<T>(true, raw.Status, value, "");
            }
            catch (Exception exception)
            {
                return new ApiResult<T>(false, raw.Status, null, "JSON lỗi: " + exception.Message);
            }
        }

        private static async Task<ApiResult<string>> SendRawAsync(UnityWebRequest request, CancellationToken cancellationToken)
        {
            try
            {
                request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return new ApiResult<string>(true, (int)request.responseCode, request.downloadHandler.text, "");
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return new ApiResult<string>(false, 0, "", "Đã huỷ");
                }

                return new ApiResult<string>(false, (int)request.responseCode, request.downloadHandler.text,
                    ExtractError(request.downloadHandler.text, (int)request.responseCode));
            }
            catch (Exception exception)
            {
                return new ApiResult<string>(false, 0, "", "Không kết nối được server: " + exception.Message);
            }
        }

        private static string ExtractError(string body, int status)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return status == 401 ? "Sai tên đăng nhập hoặc mật khẩu" : "Lỗi máy chủ (" + status + ")";
            }

            string text = body.Trim();
            if (text.StartsWith("\"") && text.EndsWith("\"") && text.Length > 2)
            {
                return text.Substring(1, text.Length - 2);
            }

            if (text.StartsWith("{"))
            {
                return status == 400 ? "Dữ liệu không đúng ràng buộc" : "Lỗi máy chủ (" + status + ")";
            }

            return text.Length > 160 ? text.Substring(0, 160) : text;
        }

        [Serializable]
        private sealed class RegisterPayload
        {
            public string username;
            public string password;
        }

        [Serializable]
        private sealed class LoginPayload
        {
            public string username;
            public string password;
        }

        [Serializable]
        private sealed class CreatePayload
        {
            public string name;
            public string @class;
        }

        [Serializable]
        private sealed class UpdatePayload
        {
            public string name;
            public string @class;
            public int level;
            public int hp;
        }
    }
}
