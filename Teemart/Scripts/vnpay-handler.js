// VNPay payment handler
function validateVNPayForm() {
    // Validate required fields
    var hoTen = $("input[name='hotennguoinhan']").val()
    var soDienThoai = $("input[name='sodienthoainhan']").val()
    var diaChi = $("input[name='diachinhan']").val()

    if (!hoTen || !soDienThoai || !diaChi) {
        alert("Vui lòng ?i?n ??y ?? thông tin nh?n hàng tr??c khi thanh toán!")
        return false
    }

    return true
}

$(document).ready(() => {
    // Handle VNPay payment button click
    $("form[action*='PaymentWithVNPay']").on("submit", function (e) {
        if (!validateVNPayForm()) {
            e.preventDefault()
            return false
        }

        // Copy form data from the main form to the VNPay form
        var hoTen = $("input[name='hotennguoinhan']").val()
        var soDienThoai = $("input[name='sodienthoainhan']").val()
        var diaChi = $("input[name='diachinhan']").val()
        var ghiChu = $("textarea[name='ghichu']").val()

        // Add hidden fields to the VNPay form
        $(this).append('<input type="hidden" name="hotennguoinhan" value="' + hoTen + '">')
        $(this).append('<input type="hidden" name="sodienthoainhan" value="' + soDienThoai + '">')
        $(this).append('<input type="hidden" name="diachinhan" value="' + diaChi + '">')
        $(this).append('<input type="hidden" name="ghichu" value="' + ghiChu + '">')

        return true
    })
})
