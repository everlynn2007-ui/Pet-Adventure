# GDD — Pet Adventure: Thế Giới Thú Cưng

Phiên bản 0.1 — Giai đoạn 1 (chưa có gameplay).
Môn: GAM106 — Lập trình Game Back-End, ASM 1.

## 1. Tóm tắt

| Mục | Nội dung |
|---|---|
| Thể loại | Pet RPG, 2D top-down |
| Nền tảng | PC (Windows), bàn phím + chuột |
| Engine | Unity 6000.3.24f1, URP 2D, Input System |
| Đồ hoạ | Cute Pixel Art, bảng màu pastel, UI bo góc |
| Backend | ASP.NET Core 8 Web API + MySQL 8 |
| Chế độ | Một người chơi, có tài khoản trực tuyến |
| Ngôn ngữ | Tiếng Việt |

Người chơi là **nhà huấn luyện linh thú**. Họ đăng ký tài khoản, đăng nhập, rồi
thu thập và quản lý nhiều linh thú. Mỗi linh thú có nguyên tố, cấp và máu riêng.
Linh thú đi theo người chơi trong làng Greenleaf.

## 2. Vòng lặp chơi

1. Đăng nhập (hoặc đăng ký) để vào game.
2. Nhận linh thú đầu tiên.
3. Đi trong Greenleaf Village, nói chuyện NPC.
4. Thu thập thêm linh thú.
5. Xem danh sách, xem chi tiết, đổi linh thú đi theo.
6. Dữ liệu linh thú lưu ở MySQL qua backend.

## 3. Nguyên tố và linh thú khởi đầu

Năm nguyên tố: `Fire`, `Water`, `Nature`, `Lightning`, `Earth`.

| Nguyên tố | Linh thú | Ngoại hình | Màu | Đặc điểm |
|---|---|---|---|---|
| Fire | **FireFox** | cáo lửa, khăn đỏ, đuôi lửa | đỏ cam | sát thương cao, máu thấp |
| Water | **Aqua** | mèo nước, vòng nước quanh chân | xanh dương | cân bằng, hồi phục |
| Nature | **Leafy** | thú lá, tai lá, dây leo | xanh lá | máu cao, chậm |
| Lightning | **Lumi** | mèo sấm, tia điện tím | tím vàng | nhanh, giòn |
| Earth | **Terran** | thú đá, giáp đất, lá trên đầu | nâu vàng | máu rất cao, chậm |

Mô tả hiển thị ở màn Pet Detail:

- FireFox — "Tinh linh lửa nhỏ bé, luôn mang ý chí chiến đấu."
- Aqua — "Tinh linh nước, mang lại sự bình yên."
- Leafy — "Tinh linh thiên nhiên, bảo vệ khu rừng xanh."
- Lumi — "Tinh linh sấm sét, nhanh nhẹn và tinh nghịch."
- Terran — "Tinh linh đất, vững chãi và mạnh mẽ."

Tương khắc (dùng từ Giai đoạn 4, chưa có ở ASM 1):
`Fire` mạnh hơn `Nature`, `Nature` mạnh hơn `Water`, `Water` mạnh hơn `Fire`.
`Lightning` trung tính, không nằm trong vòng.

## 4. Chỉ số linh thú

| Chỉ số | Kiểu | Giá trị | Ghi chú |
|---|---|---|---|
| Name | chuỗi | 2–50 ký tự | tên riêng, không trùng bắt buộc |
| Element (`Class`) | chuỗi | 1–30 ký tự | Fire / Water / Nature / Lightning |
| Level | int | 1–100 | mặc định 1 |
| Hp | int | 0–100000 | máu hiện tại, mặc định 100 |
| CreatedAt | datetime | UTC | thời điểm tạo |
| AccountId | int | FK | chủ sở hữu |

Máu tối đa suy ra từ cấp: `MaxHp = 100 + 50 × (Level − 1)`. Cấp 1 = 100,
cấp 100 = 5050. Trần 100000 của cột đủ chỗ cho mọi cấp.

Chỉ số được tính ở client (Unity) cho phần hiển thị. Backend chỉ lưu `Level`
và `Hp`, không tính toán — đúng phạm vi ASM 1.

## 5. Màn hình

| # | Màn hình | Nội dung | Nguồn dữ liệu |
|---|---|---|---|
| 1 | Main Menu | Play, Login, Settings, Quit | local |
| 2 | Login | username, password, thông báo lỗi | API auth |
| 3 | Register | username, password, xác nhận | API auth |
| 4 | Gameplay HUD | tên người chơi, linh thú đang theo, nút Pause | local |
| 5 | Pet Collection | lưới thẻ: tên, nguyên tố, cấp, thanh HP | `GET /api/characters` |
| 6 | Pet Details | đủ chỉ số, mô tả, nút "Đi theo", nút "Đổi tên" | dữ liệu collection |
| 7 | Inventory | vật phẩm: thức ăn, đá tiến hoá, bóng bắt thú | local |
| 8 | Settings | âm lượng, độ phân giải, toàn màn hình | local |
| 9 | Pause Menu | Tiếp tục, Settings, Về Main Menu | local |

Inventory không có bảng trong ASM 1 nên lưu local. Không đẩy lên server ở
phiên bản này.

### Bảng màu UI (có ngữ nghĩa)

| Nhóm | Màu | Dùng cho | Màu chữ |
|---|---|---|---|
| Nền panel | kem `#FFF0D2`, viền nâu `#8B5E3C` | khung màn hình, thẻ linh thú | nâu `#4A3728` |
| Xanh lá `#5FAE68` | xác nhận: Chơi, Đăng nhập, Lưu tên | trắng |
| Xanh dương `#6FA8DC` | điều hướng: Đăng ký, Quay lại, Linh thú, Cài đặt | trắng |
| Cam `#F0A05A` | hành động đặc biệt: Bắt thú, Đi theo, Túi đồ | nâu đậm |
| Đỏ `#D96C6C` | thoát, về Main Menu, lỗi | trắng |
| Vàng `#E8B65A` | tùy chọn: Cài đặt, Tạm dừng | nâu đậm |
| Nguyên tố | Lửa `#F4784A`, Nước `#5AA9E6`, Lá `#7FB77E`, Sấm `#E4C55A`, Đất `#B08968` | chip nguyên tố, icon, than HP | nâu đậm |

Màu nằm ở `Image.color`, không nạp cứng trong texture — tránh panel bị trắng.

## 6. Bản đồ — Greenleaf Village

- Làng nhỏ, camera top-down, người chơi đi bằng WASD / phím mũi tên.
- Khu vực: quảng trường giữa làng, nhà huấn luyện, hồ nước, rừng ven làng.
- NPC: Trưởng làng (hướng dẫn), Bác sĩ thú y (hồi máu), Thương nhân (inventory).
- Linh thú đang chọn đi theo sau người chơi, giữ khoảng cách, không cản đường.
- Chưa có chiến đấu ở phiên bản này.

## 7. Ánh xạ sang backend ASM 1

| Khái niệm game | Backend |
|---|---|
| Nhà huấn luyện | bảng `Accounts` |
| Linh thú | bảng `Characters` |
| Thu thập linh thú | `POST /api/characters` |
| Đổi tên / nâng cấp | `PUT /api/characters/{id}` |
| Danh sách linh thú | `GET /api/characters` (thêm, ngoài phạm vi ASM 1) |
| Đăng ký / đăng nhập | `POST /api/auth/register`, `POST /api/auth/login` |
| Đăng xuất | xoá token ở client |

Một tài khoản có nhiều linh thú; mỗi linh thú thuộc đúng một tài khoản.
Xoá tài khoản thì xoá linh thú (cascade) — chỉ là ràng buộc DB, chưa có API xoá.

## 8. Ngoài phạm vi (non-goals)

- Chiến đấu theo lượt, kỹ năng, tiến hoá.
- Trao đổi, chợ, nhiều người chơi.
- Phân quyền Admin/Player, refresh token, quên mật khẩu.
- Đồng bộ inventory lên server.
- Mobile, macOS, Linux.

## 9. Tiêu chí hoàn thành từng giai đoạn

| Giai đoạn | Xong khi |
|---|---|
| 1 | Có thư mục chuẩn, `AGENTS.md`, GDD, môi trường đã kiểm |
| 2 | 9 màn hình mở được, điều hướng đúng, dữ liệu giả | xong |
| 3 | Đi lại trong Greenleaf, camera bám, NPC nói chuyện, linh thú đi theo | xong, chưa play test tay |
| 4 | 5 nguyên tố, `PetData` ScriptableObject, danh sách + chi tiết hiển thị đúng | xong |
| 5 | 4 API chạy, đủ TC01–TC12 đúng mã HTTP, ảnh chụp đủ | xong, ảnh Swagger đủ |
| 6 | Unity đăng nhập bằng API thật, tạo/sửa linh thú ghi vào MySQL, có ảnh báo cáo | code xong, play test + ảnh Workbench chưa |
