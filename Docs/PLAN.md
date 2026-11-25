# Kế hoạch thực hiện — Pet Adventure

Trạng thái hiện tại: Giai đoạn 1–5 xong (kể cả 12 ảnh Swagger); Giai đoạn 6 có
code nhưng chưa play test trong Unity Editor và chưa chụp ảnh MySQL Workbench.
Chi tiết: `Docs/VERIFICATION.md`.

Kế hoạch chạy theo 6 giai đoạn, không nhảy bước. Mỗi giai đoạn có kết quả nhìn
thấy được và cách kiểm chứng.

## 1. Chốt kỹ thuật

| Hạng mục | Chọn | Lý do |
|---|---|---|
| Unity | 6000.3.24f1, URP 2D, Input System, uGUI | có sẵn trên máy |
| Backend | ASP.NET Core 8 (`net8.0`) | đúng bản demo của đề: Pomelo 8.0.3 + EF 8.0.31 |
| SDK khi build | pin `9.0.313` bằng `global.json` | máy có cả SDK 10, 11 |
| DB | MySQL 8, service `MySQL2610`, DB `game_pet_adventure` | |
| Auth | JWT Bearer, HS256, hạn 1 ngày | |
| Unity ↔ API | `UnityWebRequest` + `JsonUtility` + wrapper list | không thêm package |
| Inventory | local (ScriptableObject + save file) | ASM 1 không có bảng item |

## 2. Cấu trúc thư mục

```
D:\Unity\Pet Adventure\
  Assets\_Project\Scripts\{Core,UI,Gameplay,Pets,Networking}
  Assets\_Project\{Scenes,Prefabs,ScriptableObjects,Art,Audio}
  Backend\GameApi\        # ASP.NET Core, ngoài Assets
  Docs\{GDD,DatabaseDesign,API_Documentation,PLAN}.md
  Docs\ASM1\              # ảnh TC01..TC12, ảnh bảng MySQL
  AGENTS.md
```

Client và backend tách rời: client chỉ biết DTO + endpoint, không reference
project backend.

## 3. Giai đoạn và tiêu chí xong

### Giai đoạn 1 — Khởi tạo (xong)
- Kiểm môi trường: .NET SDK 9.0.313, runtime 8.0.34, dotnet-ef 9.0.23,
  Unity 6000.3.24f1, MySQL running.
- Cây thư mục, `AGENTS.md`, `GDD.md`, `DatabaseDesign.md`,
  `API_Documentation.md`, `.gitignore`.
- Xong khi: mọi file trên tồn tại, chưa có code gameplay.

### Giai đoạn 2 — UI (Prompt 2)
9 màn: Main Menu, Login, Register, HUD, Pet Collection, Pet Details, Inventory,
Settings, Pause.
- Component dùng lại: `PanelBase`, `PrimaryButton`, `InputFieldRow`, `PetCard`.
- Canvas `CanvasScaler` ScaleWithScreenSize 1920×1080, Match 0.5.
- Placeholder sprite sinh bằng code (hình chữ nhật bo góc, bảng màu pastel).
- Editor script `Tools/Pet Adventure/Build UI Scenes` sinh scene + prefab.
- Xong khi: mở từng scene, bấm nút điều hướng đúng, không lỗi Console.

### Giai đoạn 3 — Gameplay top-down (Prompt 3)
- `PlayerMovement` (Rigidbody2D, `MovePosition` trong `FixedUpdate`),
  `PlayerAnimator`, `CameraFollow` (LateUpdate, smoothdamp),
  `Interactable` + `PlayerInteractor` (phím E, `Physics2D.OverlapCircle`),
  `PetCompanion` (follow có khoảng cách, không đè người chơi),
  `Collectible`.
- Map Greenleaf Village bằng Tilemap: đường đá, nhà, hồ nước, cây.
- Xong khi: đi lại mượt, va chạm đúng, camera bám, NPC hiện prompt E, nhặt được
  item, linh thú đi theo không rung.

### Giai đoạn 4 — Hệ linh thú (Prompt 4)
- `PetDefinition` (ScriptableObject): id, name, class, description, icon,
  sprite, base stats.
- `PetRuntimeData` (class thường, serializable): id, name, class, level, hp,
  maxHp suy ra.
- `PetCollection` (runtime, không phải MonoBehaviour): thêm, chọn, đổi tên,
  lưu local.
- 4 asset: FireFox (Fire), Aqua (Water), Leafy (Nature), Lumi (Lightning).
- Xong khi: màn Collection hiện đủ thẻ, click ra Details đúng chỉ số, đổi tên
  cập nhật cả 2 màn.

### Giai đoạn 5 — Backend ASM 1 (Prompt 5)
- 5 lớp: Models, Data, DTOs, Services, Controllers.
- 4 endpoint bắt buộc + `GET /api/characters`.
- Migration `InitialCreate`, DB `game_pet_adventure`.
- Swagger có nút Authorize.
- Xong khi: TC01–TC12 đúng mã HTTP, ảnh chụp đủ 12 ca, ảnh 2 bảng MySQL.

### Giai đoạn 6 — Kết nối (Prompt 6)
- `ApiClient`: register, login, getPets, createPet, updatePet.
- Login thật từ Unity → lưu token → tải collection từ MySQL.
- Tạo/sửa linh thú từ UI → ghi DB.
- Xong khi: đăng nhập bằng tài khoản tạo trên Swagger, thấy linh thú thật;
  tạo mới trong Unity rồi thấy trong MySQL Workbench.

## 4. Thứ tự thực hiện tiếp theo

1. Giai đoạn 2: dựng khung UI + 3 màn đầu (Main Menu, Login, Register) với dữ
   liệu giả.
2. Làm nốt 6 màn còn lại, kiểm từng màn.
3. Giai đoạn 3 → 4 → 5 → 6 theo bảng trên.

## 5. Rủi ro và cách xử lý

| Rủi ro | Xử lý |
|---|---|
| Tôi không thao tác được trong Unity Editor | sinh scene/prefab bằng editor script, bạn bấm menu một lần |
| Lệch phiên bản gói EF/Pomelo | pin đúng bộ 8.0.3 / 8.0.31 |
| Máy có SDK 10, 11 | `global.json` pin 9.0.313 |
| Chưa biết mật khẩu MySQL | hỏi bạn trước khi chạy migration |
| Ảnh tham khảo có Terran (Đất) | đã chốt giữ cả 5 nguyên tố: Fire, Water, Nature, Lightning, Earth |
| Ảnh UI tham khảo có Shop, Quest, Equipment | ngoài phạm vi ASM 1, không làm |
| Không có sprite thật | placeholder sinh bằng code, thay sau |
| git chưa có trong PATH | dùng git bundled của GitHub Desktop, hoặc thêm PATH |

## 6. Những gì không làm

- Chiến đấu, tiến hoá, trao đổi, nhiều người chơi.
- Đồng bộ inventory lên server.
- Bảng hay endpoint ngoài danh sách ở `API_Documentation.md`.
- Sinh toàn bộ gameplay trong một lần.
