using PetAdventure.Networking;
using PetAdventure.Pets;

namespace PetAdventure.Core
{
    /// <summary>Trạng thái phiên chơi. UI đọc từ đây, không đọc trực tiếp API.</summary>
    public sealed class GameState
    {
        public readonly PetCollection Pets = new PetCollection();
        public readonly string ServerUrl = "http://localhost:5080";

        public bool UseBackend { get; set; }

        public ApiClient Api { get; }

        public GameState()
        {
            Api = new ApiClient(ServerUrl);
        }

        public string Username { get; private set; } = "";
        public string Token { get; private set; } = "";
        public int AccountId { get; private set; }

        public bool IsLoggedIn => Token.Length > 0;

        public int SelectedPetId { get; set; }
        public int CompanionPetId { get; set; }

        public float MasterVolume = 0.8f;
        public bool Fullscreen = true;
        public int CollectedItems { get; set; }

        public PetRuntimeData Companion => Pets.Get(CompanionPetId);

        public void SignIn(string username, string token, int accountId = 0)
        {
            Username = username ?? "";
            Token = token ?? "";
            AccountId = accountId;
            Api.Token = Token;
        }

        public void SignOut()
        {
            Username = "";
            Token = "";
            AccountId = 0;
            Api.Token = "";
            SelectedPetId = 0;
            CompanionPetId = 0;
            Pets.Clear();
        }

        /// <summary>Dữ liệu demo cho Giai đoạn 2. Giai đoạn 6 thay bằng dữ liệu API.</summary>
        public void SeedDemoIfEmpty()
        {
            if (Pets.Count == 0)
            {
                Pets.SeedDemo(PetElementInfo.All.Length);
            }
        }
    }
}
