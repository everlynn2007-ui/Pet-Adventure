# Verification — Pet Adventure

Ngày chạy: 25/11/2026. Mọi dòng dưới đây là kết quả thật từ lệnh đã chạy.

## 1. Môi trường

| Thứ | Kết quả |
|---|---|
| `dotnet --list-sdks` | 9.0.313, 10.0.403, 11.0.100 |
| `dotnet --list-runtimes` | ASP.NET Core 8.0.34 + 9.0.15 |
| `dotnet ef --version` | 9.0.23 |
| Unity Editor | 6000.3.24f1, URP 2D, Input System 1.20.0 |
| MySQL | service `MySQL2610`, `SELECT VERSION()` = `26.10.0`, user `root` / `123456` |
| git | không có trong PATH; repo đã init, chưa commit |

## 2. Unity

Không chạy batchmode trực tiếp trên project chính vì project đang mở trong Editor.
Copy `Assets`, `Packages`, `ProjectSettings` sang
`C:\Users\ADMIN\AppData\Local\Temp\opencode\PetAdventureCheck`, rồi chạy Unity batchmode:

| Lệnh | Kết quả |
|---|---|
| `BuildProject` | `EXIT=0`, sinh 5 file `Pet_*.asset`, PNG icon/world, scene `PetAdventure.unity` |
| `SelfCheck` | `EXIT=0`, `SELFCHECK PASS screens=9 buttons=29 labels=88 pets=5 npcs=3 items=5` |
| Compile | `0 error CS` sau 3 lần sửa lỗi (xem bên dưới) |

Lỗi đã sửa trong quá trình compile:
- `CS7036` — thiếu tham số `style` ở `PetDetailsScreen.PlaceLeft`.
- `CS0119` — `Slider.Direction` bị hiểu là method `UiFactory.Slider`; đổi sang `UnityEngine.UI.Slider.Direction`.
- `Texture2D.SetPixels` — mảng 4 phần tử cho texture 4×4; đổi thành 16.

Asset sinh ra đã copy về project chính. Project chính đang mở: cần focus Unity để import lại,
sau đó mở scene `Assets/_Project/Scenes/PetAdventure.unity` và nhấn Play.

## 3. Backend

| Lệnh | Kết quả |
|---|---|
| `dotnet build --nologo` | `Build succeeded. 0 Warning(s), 0 Error(s)` |
| `dotnet ef migrations add InitialCreate --no-build` | 3 file trong `Migrations/` |
| chạy app | `CREATE DATABASE game_pet_adventure`, tạo `Accounts`, `Characters`, `__EFMigrationsHistory`; `Now listening on: http://localhost:5080` |

Bảng `Characters` sau migration: `Id`, `Name varchar(50)`, `Class varchar(30)`,
`Level int DEFAULT 1`, `Hp int DEFAULT 100`, `CreatedAt datetime(6)`, `AccountId int`,
FK `ON DELETE CASCADE`, index `IX_Accounts_Username` unique.

## 4. TC01–TC12

Chạy `Backend/test-asm1.ps1` (cần server đang chạy):

| Ca | Mong đợi | Thực tế |
|---|---|---|
| TC01 register | 201 | 201 PASS |
| TC02 trùng username | 409 | 409 PASS |
| TC03 password 3 ký tự | 400 | 400 PASS |
| TC04 sai password | 401 | 401 PASS |
| TC05 login đúng | 200 + token | 200, token 312 ký tự |
| TC06 không token | 401 | 401 PASS |
| TC07 Authorize | có token | token dùng được |
| TC08 tạo FireFox/Fire | 201, level 1, hp 100 | 201 PASS |
| TC09 tên rỗng | 400 | 400 PASS |
| TC10 level 5, hp 250 | 200 | 200 PASS |
| TC11 level 0 | 400 | 400 PASS |
| TC12 sửa pet người khác | 404 | 404 PASS |
| GET thêm | 200 | 200 PASS |

`SCRIPT_EXIT=0`, `ASM1 RESULT: ALL PASS`.

Kiểm tra MySQL sau test:
- `PasswordHash` bắt đầu bằng `AQAAAA`, không phải `123456`.
- Dòng `FireFox / Fire / 5 / 250` tồn tại; mỗi `AccountId` khớp chủ sở hữu.

## 5. Ảnh Swagger

Đã chụp tự động bằng Playwright, từ Swagger UI thật của backend đang chạy:

- `Docs/ASM1/TC01.png` … `Docs/ASM1/TC12.png`
- `Docs/ASM1/Swagger-overview.png`

Mỗi ảnh TCxxx là ảnh chụp phần opblock tương ứng ngay sau khi Execute, chứa
Request URL, Response body và mã HTTP. Kiểm tra lại:

| Ảnh | Mã trong ảnh |
|---|---|
| TC01 | 201 |
| TC02 | 409 |
| TC03 | 400 |
| TC04 | 401 |
| TC05 | 200 + token |
| TC06 | 401 |
| TC07 | Authorized |
| TC08 | 201, level 1, hp 100 |
| TC09 | 400 |
| TC10 | 200, level 5, hp 250 |
| TC11 | 400 |
| TC12 | 404 |

## 7. Lỗi phát hiện khi mở Play trong Editor

Console báo `The referenced script (Unknown) on this Behaviour is missing!`,
Game view báo `No cameras rendering`. Nguyên nhân: scene và 5 file `.asset` được
copy từ bản temp nên GUID script trong YAML lệch với `.meta` của project thật.

Đã sửa:
- `PetAdventure.unity`: `m_Script` → guid `b09e56c42b5089040bc0943f1905a63e` (`GameBootstrap.cs.meta`).
- 5 file `Resources/Pets/Pet_*.asset`: `m_Script` → guid `5f4dd3af5cf5d9f4b83d97a187135c98` (`PetDefinition.cs.meta`).

Trong Editor: tắt Play, mở lại scene `Assets/_Project/Scenes/PetAdventure.unity`, rồi Play lại.

## 6. Chưa làm

- Chưa chụp 2 ảnh MySQL Workbench. Đã có kết quả SQL text trong mục 4 thay thế.
- Chưa bấm Play trong Unity Editor để test tay: đi lại, va chạm, NPC, bắt thú, đăng nhập thật.
  SelfCheck chỉ dựng đối tượng ở EditMode, không mô phỏng input người dùng.
- Chưa commit git (git không có trong PATH; dùng GitHub Desktop nếu cần).
