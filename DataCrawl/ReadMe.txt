Sử dụng thư viện: PuppeteerSharp,
Mở source bằng Visual Studio code or Visual Studio 2022,
Restore package rồi chạy.

Em chưa lấy được bài viết của nguyên tháng của vnexpress.vn
Hiện tại chỉ lấy được bài viết của 1 tuần, nếu muốn của nguyên tháng, thì mình cần lưu lại từng tuần.

Do dữ liệu 1 tuần quá nhìu nên tạm thời em không có đọc hết tất cả bài viết,
mà em chỉ lấy ra 10 bài viết có nhìu bình luận nhất. 
Sau đó e đọc chi tiết của mỗi bài viết để lấy ra số lượng like của mỗi dòng bình luận,
rồi em sum lại và bắt đầu xắp xếp thứ tự của 10 bài viết đó.

Em chưa lấy được bài viết theo ngày của tuoitre.vn

Câu 1, 2
Dùng thư viện undercore.js để lọc data

Câu 3: Em mở các gửi các yêu cầu lên các server để nhận dữ liệu về, yêu cầu nào dc server xử lý trả dữ liệu về trước thì e sẽ lọc data từ luồng đó 
cho đến khi toàn bộ yêu cầu gửi lên server dc xử lý xong.

VD: Server thứ nhất trả về và em lọc ra dc 20 số không trùng lặp, sau đó em nối 20 số đó vào dữ liệu dc trả ra từ server thứ hai, 
sau đó e lại lặp lại bước lọc để lấy ra danh sách không trùng lặp. 
Việc lấy dữ liệu không trùng lặp bao nhiêu lần, phụ thuộc vào có bao nhiêu server và việc thời gian server đó trả dữ liệu về khi nào.
