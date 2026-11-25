# Pet Adventure — Hướng dẫn cho Agent

Game 2D top-down pet RPG. Unity `6000.3.24f1` (URP 2D, Input System, uGUI) +
backend ASP.NET Core 8 + MySQL 8. Đây là đồ án GAM106 — ASM 1, chấm theo
`Hướng Dẫn ASM 1 - Game API ASP.NET Core.pdf`.

## Trạng thái giai đoạn

| Giai đoạn | Nội dung | Trạng thái |
|---|---|---|
| 1 | Khởi tạo: môi trường, thư mục, AGENTS.md, GDD | xong |
| 2 | Giao diện: Main Menu, Login, Register, HUD, Pet Collection, Pet Details, Inventory, Settings, Pause | xong |
| 3 | Gameplay: di chuyển top-down, camera, Greenleaf Village, NPC, pet follow | xong |
| 4 | Hệ linh thú: 5 nguyên tố, PetData, Level/HP, danh sách, hiển thị | xong |
| 5 | Backend ASM 1: Web API, MySQL, EF Core, JWT, TC01–TC12 | xong |
| 6 | Kết nối Unity ↔ API, lưu DB, kiểm lỗi, tài liệu + ảnh báo cáo | code xong, chưa play test tay; ảnh Workbench chưa |

Không nhảy giai đoạn. Giai đoạn 1 không viết gameplay.

## Cấu trúc

```
D:\Unity\Pet Adventure\
  Assets\_Project\
    Scripts\Core        # ApiClient, SaveService, GameState
    Scripts\UI          # MainMenu, Auth, Collection, Detail, Inventory
    Scripts\Gameplay    # PlayerMovement, CameraFollow, NPC, PetFollow
    Scripts\Pets        # PetData (ScriptableObject), PetElement, PetStats
    Scripts\Networking  # DTO khớp backend, AuthSession
    Scenes\             # MainMenu, Village, ...
    Prefabs\  ScriptableObjects\  Art\  Audio\
  Backend\GameApi\      # ASP.NET Core — ngoài Assets, Unity không import
  Docs\GDD.md
  Docs\DatabaseDesign.md
  Docs\API_Documentation.md
  Docs\PLAN.md
  Docs\VERIFICATION.md
  Docs\ASM1\            # ảnh TC01..TC12 + ảnh 2 bảng MySQL
```

`Backend/` nằm ngoài `Assets/` nên Unity bỏ qua. Không đặt file `.cs` backend
trong `Assets/`.

## Ràng buộc backend ASM 1 — không được lệch

- API bắt buộc: `POST /api/auth/register`, `POST /api/auth/login`,
  `POST /api/characters`, `PUT /api/characters/{id}`.
- `GET /api/characters` là phần thêm cho màn Pet Collection. Ghi chú rõ
  "ngoài phạm vi ASM 1" trong báo cáo. Không thêm API khác.
- Đúng 2 bảng: `Accounts`, `Characters`. Không thêm bảng, không thêm cột.
- `Class` = chuỗi 1–30 ký tự. Giá trị dùng trong game: `Fire`, `Water`,
  `Nature`, `Lightning`, `Earth`.
- `Level` 1–100, `Hp` 0–100000, mặc định khi tạo: 1 và 100.
- `Level`/`Hp` trong `UpdateCharacterRequest` phải là `int?` + `[Required]`.
- Mật khẩu băm bằng `PasswordHasher`, không API nào trả `PasswordHash`.
- `AccountId` luôn lấy từ claim `NameIdentifier`, không lấy từ body/URL.
- Trùng username → 409; sai đăng nhập → 401 (cùng một thông báo cho cả sai
  username và sai mật khẩu); sửa nhân vật người khác → 404.
- Không phân quyền, không refresh token, không email.
- Chia lớp ngang: `Models / Data / DTOs / Services / Controllers`. Controller
  không dùng `DbContext` trực tiếp.

## Bộ gói backend (đúng bản demo, không tự nâng major)

| Gói | Bản |
|---|---|
| `Pomelo.EntityFrameworkCore.MySql` | 8.0.3 |
| `Microsoft.EntityFrameworkCore.Design` | 8.0.31 |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.31 |
| `Swashbuckle.AspNetCore` | 6.9.0 |
| Target framework | `net8.0` |

`global.json` pin SDK `9.0.313` để SDK 10/11 không chen vào.

## Quy ước code

- Namespace Unity: `PetAdventure.Core`, `PetAdventure.UI`, … theo thư mục.
- Logic gọi API nằm trong class C# thuần, không nhét vào `MonoBehaviour`.
- Dùng `UnityWebRequest`; `JsonUtility` không đọc mảng ở cấp cao nhất → bọc
  danh sách bằng class wrapper (`CharacterListResponse`).
- Token lưu trong bộ nhớ phiên + `PlayerPrefs` cho lần chạy sau.
- Text UI tiếng Việt, identifier và comment code tiếng Anh.
- Không commit `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `bin/`, `obj/`.

## Lệnh

```powershell
# Backend
cd "D:\Unity\Pet Adventure\Backend\GameApi"
dotnet build
dotnet ef migrations add <Ten>
dotnet ef database update
dotnet run
# Swagger: http://localhost:5xxx/swagger
```

Unity: mở project bằng Unity Hub, hoặc
`& "C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe" -projectPath "D:\Unity\Pet Adventure"`.

## Kiểm chứng

- Giai đoạn 5 chỉ coi là xong khi đủ 12 ca TC01–TC12 đúng mã HTTP.
- Giai đoạn 6 chỉ coi là xong khi đăng nhập bằng tài khoản thật từ Unity và
  dữ liệu linh thú nằm trong MySQL.
- Không báo "xong" khi chưa chạy lệnh kiểm tra và thấy kết quả.
