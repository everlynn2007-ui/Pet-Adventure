# Database Design — Pet Adventure

MySQL 8, tạo bằng EF Core migration. Không viết SQL tay, không thêm bảng.
Phạm vi: ASM 1 — GAM106.

## 1. Tổng quan

- Database: `game_pet_adventure`
- Charset: `utf8mb4` (tên linh thú có tiếng Việt)
- Collation: `utf8mb4_unicode_ci`
- Quan hệ: `Accounts` 1 — n `Characters`
- Xoá tài khoản → xoá toàn bộ linh thú của tài khoản (cascade)

```
Accounts (1) ────< (n) Characters
  Id (PK)               Id (PK)
  Username (UQ)         AccountId (FK → Accounts.Id, CASCADE)
  PasswordHash          Name, Class, Level, Hp
  CreatedAt             CreatedAt
```

## 2. Bảng Accounts

| Cột | Kiểu | Null | Ràng buộc | Ghi chú |
|---|---|---|---|---|
| Id | int | không | PK, auto increment | |
| Username | varchar(50) | không | unique index, 3–50 ký tự | kiểm độ dài bằng DTO |
| PasswordHash | varchar(255) | không | | PBKDF2 của `PasswordHasher`, bắt đầu `AQAAAA` |
| CreatedAt | datetime | không | | UTC, gán trong Service |

Index: `IX_Accounts_Username` unique.

Không lưu mật khẩu gốc. Không có cột Role — ASM 1 không phân quyền.

## 3. Bảng Characters

| Cột | Kiểu | Null | Mặc định | Ràng buộc | Ghi chú |
|---|---|---|---|---|---|
| Id | int | không | | PK, auto increment | |
| Name | varchar(50) | không | | 2–50 ký tự (DTO) | tên linh thú |
| Class | varchar(30) | không | | 1–30 ký tự (DTO) | Fire / Water / Nature / Lightning / Earth |
| Level | int | không | 1 | 1–100 khi sửa | |
| Hp | int | không | 100 | 0–100000 khi sửa | máu hiện tại |
| CreatedAt | datetime | không | | | UTC, gán trong Model |
| AccountId | int | không | | FK → Accounts.Id, ON DELETE CASCADE | chủ sở hữu |

Index: `IX_Characters_AccountId` (EF tự tạo cho FK).

`Class` là chuỗi tự do trong DB; kiểm tra thuộc 5 nguyên tố là việc của client
(Giai đoạn 4). Không thêm CHECK constraint — đề chỉ yêu cầu 1–30 ký tự.

## 4. Ánh xạ game ↔ DB

| Trường game (Prompt 4) | Cột DB | Ghi chú |
|---|---|---|
| Id | Characters.Id | |
| Name | Characters.Name | đổi tên bằng PUT |
| Class | Characters.Class | nguyên tố |
| Level | Characters.Level | |
| HP | Characters.Hp | |
| MaxHP | — | suy ra: `100 + 50 × (Level − 1)`, chỉ ở client |
| Description | — | thuộc `PetData` ScriptableObject, không lưu DB |
| Icon, Sprite | — | asset, không lưu DB |
| AccountId | Characters.AccountId | lấy từ JWT, không từ body |

## 5. Vì sao không có bảng khác

| Thứ muốn thêm | Lý do không thêm |
|---|---|
| Items / Inventory | ASM 1 chốt 2 bảng. Inventory lưu local trong Unity |
| PetSpecies (loài) | Chỉ 4 loài; đặt trong `PetData` ScriptableObject |
| Roles | ASM 1 không phân quyền |
| RefreshTokens | ASM 1 không có refresh token |

Thêm bảng là lệch yêu cầu bài và mất điểm.

## 6. Migration

```powershell
cd "D:\Unity\Pet Adventure\Backend\GameApi"
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Kết quả mong đợi trong MySQL Workbench: `accounts`, `characters`,
`__efmigrationshistory`.

Lỗi hay gặp: chạy app bằng `--no-build` ngay sau khi thêm migration → app dùng
bản build cũ → `Table 'game_pet_adventure.accounts' doesn't exist`. Sửa: build
lại.

## 7. Kiểm tra sau khi chạy TC01–TC12

- `accounts`: `PasswordHash` là chuỗi dài bắt đầu `AQAAAA`, không phải `123456`.
- `characters`: đủ bản ghi cho mỗi lần `POST`; bản ghi vừa `PUT` có Level/Hp mới.
- Mỗi `AccountId` trong `characters` khớp `accounts.Id` của người tạo.
