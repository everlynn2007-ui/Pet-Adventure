# API Documentation — Pet Adventure

Backend ASP.NET Core 8 Web API + MySQL 8. Theo ASM 1 — GAM106.

- Base URL local: `http://localhost:5080` (`Properties/launchSettings.json`)
- Swagger UI: `/swagger`
- JSON: camelCase, mặc định của `System.Text.Json`
- Xác thực: `Authorization: Bearer <token>`

## 1. Danh sách endpoint

| # | Method | Đường dẫn | Token | Mã thành công |
|---|---|---|---|---|
| 1 | POST | `/api/auth/register` | không | 201 |
| 2 | POST | `/api/auth/login` | không | 200 |
| 3 | POST | `/api/characters` | có | 201 |
| 4 | PUT | `/api/characters/{id}` | có | 200 |
| 5 | GET | `/api/characters` | có | 200 |

Endpoint 5 là phần thêm cho màn Pet Collection. Ghi chú "ngoài phạm vi ASM 1"
trong báo cáo. Đề yêu cầu đúng 4 endpoint đầu.

## 2. POST /api/auth/register

Nhận: `RegisterRequest`

```json
{ "username": "trainer01", "password": "123456" }
```

Ràng buộc DTO: `Username` `[Required, StringLength(3,50)]`,
`Password` `[Required, StringLength(6,100)]`.

Thành công — 201:

```json
{ "id": 1, "username": "trainer01" }
```

| Mã | Khi nào | Body |
|---|---|---|
| 400 | vi phạm ràng buộc DTO | ProblemDetails, lỗi ở `errors.Password` / `errors.Username` |
| 409 | username đã tồn tại | `"Tên đăng nhập đã tồn tại"` |

Không bao giờ trả `PasswordHash`.

## 3. POST /api/auth/login

Nhận: `LoginRequest` — cùng hình dạng body, đăng nhập không giới hạn độ dài
chặt như register (chỉ `[Required]`).

Thành công — 200:

```json
{ "token": "eyJhbGciOiJIUzI1NiIs...", "username": "trainer01" }
```

| Mã | Khi nào | Body |
|---|---|---|
| 400 | thiếu trường | ProblemDetails |
| 401 | sai username **hoặc** sai mật khẩu | `"Sai tên đăng nhập hoặc mật khẩu"` |

Thông báo 401 phải giống nhau cho cả hai trường hợp, để không dò được username
nào tồn tại.

Token: `ClaimTypes.NameIdentifier` = `Account.Id`, `ClaimTypes.Name` = username,
hạn 1 ngày, ký `HmacSha256` bằng `Jwt:Key` (≥ 32 ký tự).

## 4. POST /api/characters

Header: `Authorization: Bearer <token>`. Nhận: `CreateCharacterRequest`

```json
{ "name": "FireFox", "class": "Fire" }
```

`Name` `[Required, StringLength(50, MinimumLength = 2)]`,
`Class` `[Required, StringLength(30, MinimumLength = 1)]`.

Server tự đặt `Level = 1`, `Hp = 100`, `CreatedAt = DateTime.UtcNow`,
`AccountId` = claim `NameIdentifier`.

Thành công — 201:

```json
{ "id": 2, "name": "FireFox", "class": "Fire", "level": 1, "hp": 100, "accountId": 1 }
```

| Mã | Khi nào |
|---|---|
| 400 | name/class sai ràng buộc |
| 401 | thiếu / sai / hết hạn token |

## 5. PUT /api/characters/{id}

Header: `Authorization: Bearer <token>`. Nhận: `UpdateCharacterRequest`

```json
{ "name": "FireFox", "class": "Fire", "level": 5, "hp": 250 }
```

`Level` và `Hp` **bắt buộc là `int?` + `[Required]`**. Nếu dùng `int` thường,
client thiếu `hp` sẽ bị điền 0 và API trả 200 — linh thú mất sạch máu.

Ràng buộc: `Name` 2–50, `Class` 1–30, `Level` `[Range(1,100)]`,
`Hp` `[Range(0,100000)]`.

Thành công — 200: trả `CharacterResponse` sau khi sửa.

| Mã | Khi nào | Body |
|---|---|---|
| 400 | sai ràng buộc (ví dụ `level: 0`) | ProblemDetails |
| 401 | thiếu / sai token | |
| 404 | không có nhân vật **hoặc** của tài khoản khác | `"Không tìm thấy nhân vật của bạn"` |

Trả 404 chứ không 403: người gọi không được biết nhân vật đó có tồn tại hay không.

## 6. GET /api/characters (thêm)

Header: `Authorization: Bearer <token>`. Trả mảng linh thú của chính tài khoản
trong token, sắp xếp theo `Id`.

```json
[
  { "id": 2, "name": "FireFox", "class": "Fire", "level": 5, "hp": 250, "accountId": 1 }
]
```

Mã: 200 thành công, 401 thiếu token. Không trả linh thú của tài khoản khác.

## 7. Mã lỗi tổng hợp

| Mã | Ai trả | Nghĩa |
|---|---|---|
| 200 | Controller | đăng nhập / sửa / lấy danh sách thành công |
| 201 | Controller | đăng ký / tạo linh thú thành công |
| 400 | ASP.NET Core (DTO) | dữ liệu sai ràng buộc |
| 401 | Controller hoặc JwtBearer | sai đăng nhập; thiếu / sai / hết hạn token |
| 404 | Controller | linh thú không có hoặc không phải của bạn |
| 409 | Controller | username đã tồn tại |

## 8. Cách test trên Swagger

1. Chạy `dotnet run`, mở `/swagger`.
2. `POST /api/auth/register` → 201.
3. `POST /api/auth/login` → 200, copy `token` (không lấy dấu ngoặc kép).
4. Bấm **Authorize**, dán token (không gõ thêm chữ `Bearer`), Authorize → Close.
5. `POST /api/characters` → 201, level 1, hp 100.
6. `PUT /api/characters/{id}` → 200 với level/hp mới.
7. `GET /api/characters` → 200, thấy linh thú vừa tạo.

## 9. Ánh xạ TC01–TC12

| Ca | Endpoint | Dữ liệu | Mong đợi |
|---|---|---|---|
| TC01 | register | trainer01 / 123456 | 201 |
| TC02 | register | trainer01 lần nữa | 409 |
| TC03 | register | mật khẩu `123` | 400, lỗi Password |
| TC04 | login | trainer01 / sai | 401 |
| TC05 | login | trainer01 / 123456 | 200 + token |
| TC06 | POST characters | không Authorize | 401 |
| TC07 | Authorize | dán token TC05 | Authorized |
| TC08 | POST characters | FireFox / Fire | 201, level 1, hp 100 |
| TC09 | POST characters | name rỗng | 400, lỗi Name |
| TC10 | PUT characters/{id} | level 5, hp 250 | 200 |
| TC11 | PUT characters/{id} | level 0 | 400, lỗi Level |
| TC12 | PUT characters/{id} | id của trainer02 | 404 |

## 10. Ghi chú tích hợp Unity

- Dùng `UnityWebRequest`; `Content-Type: application/json`.
- `JsonUtility` không đọc mảng ở cấp cao nhất → bọc `GET` bằng wrapper
  `{ "items": [...] }` ở client, hoặc dùng Newtonsoft. Chọn wrapper để không
  thêm package.
- Tên trường trong class DTO của Unity phải trùng JSON camelCase
  (`accountId`, `level`, `hp`).
- Token giữ trong bộ nhớ phiên, gắn header `Authorization` cho mọi request cần
  token. Gặp 401 → xoá token, quay về màn Login.
