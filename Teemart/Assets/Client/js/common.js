//Load sản phẩm lên modal
function loadSanPham(id) {
    $.ajax({
        type: 'POST',
        data: { "id": id },
        url: '/Product/Index',
        success: function (response) {
            $("#modal-a-hinhanh").attr("href", response.HinhAnh);
            $("#modal-hinhanh").attr("src", response.HinhAnh);
            $("#modal-tensp").html(response.TenSP);
            $("#modal-danhmuc").html(response.DanhMuc.TenDanhMuc);
            $("#modal-gia").html(response.Gia.toLocaleString('it-IT', { style: 'currency', currency: 'VND' }));
            $("#modal-mamau").val(response.MaMau.trim());
            $.each(response.SanPhamChiTiets, function (index) {
                $("#kichco-soluong-" + response.SanPhamChiTiets[index].MaKichCo).val(response.SanPhamChiTiets[index].IDCTSP);
            })
            if (response.SanPhamChiTiets[0].SoLuong == 0) {
                $("#order-text").html("Hết hàng ! Hãy chọn kích cỡ khác");
                $("#order-text").attr("disabled", "disabled");
            }
        },
        error: function (response) {
            //debugger;  
            console.log(xhr.responseText);
            alert("Error has occurred..");
        }
    });
}

$(document).on("change", "#modal-kichco-soluong", function () {
    let id = $(this).val();
    console.log("Size changed:", id); // kiểm tra xem có vào đây không

    $.ajax({
        type: 'POST',
        data: { "id": id },
        url: '/Product/Detail',
        success: function (response) {
            console.log("AJAX success:", response);
            if (response.SoLuong > 0) {
                $("#order-text").html("Thêm vào giỏ").removeAttr("disabled");
            } else {
                $("#order-text").html("Hết hàng ! Hãy chọn kích cỡ khác").attr("disabled", "disabled");
            }
        },
        error: function (xhr, status, error) {
            console.error("AJAX error:", xhr.responseText);
            alert("Error has occurred..");
        }
    });
});

//Ajax thêm sp vào giỏ hàng
function themVaoGioHang() {
    let idctsp = $("#modal-kichco-soluong").val();
    let soluong = $("#modal-soluong").val();
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ "idctsp": idctsp, "soluongmua": soluong }),
        url: '/Cart/AddToCart',
        success: function (response) {
            if (!response.status) {
                swal({
                    title: "Thất bại!",
                    text: "Số lượng hàng trong kho không đủ",
                    type: "danger",
                    icon: "warning",
                    timer: 1500,
                    button: false
                });
            } else {
                $("#product-count").html(response.cart.length);
                $(".close").click();
                swal({
                    title: "Thành công!",
                    text: "Xem chi tiết tại giỏ hàng nhé <3",
                    type: "success",
                    icon: "success",
                    timer: 1500,
                    button: false
                });
            }
        },
        error: function (response) {
            //debugger;  
            console.log(xhr.responseText);
            alert("Error has occurred..");
        }
    });
}
//Ajax xóa sp trong giỏ hàng
function xoaGioHang(idctsp) {
    let total = 0;
    swal({
        title: "Bạn có chắc chắn",
        text: "Xóa sản phẩm này khỏi giỏ hàng",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    type: 'POST',
                    data: { "idctsp": idctsp },
                    url: '/Cart/DeleteFromCart',
                    success: function (response) {
                        if (response.length == 0) {
                            window.location = "/Cart/Orders";
                        } else {
                            $("#row-order-" + idctsp).remove();
                            $("#product-count").html(response.length);
                            $.each(response, function (index) {
                                total += response[index].SoLuongMua * response[index].GiaMua;
                            })
                            $("#order-total").html(total.toLocaleString('it-IT', { style: 'currency', currency: 'VND' }));
                        }
                    },
                    error: function (response) {
                        //debugger;  
                        console.log(xhr.responseText);
                        alert("Error has occurred..");
                    }
                });
            }
        });
}

//Ajax đặt hàng
function datHang() {
    let data = {};
    let formData = $('#add-bill-form').serializeArray({
    });
    $.each(formData, function (index, value) {
        data["" + value.name + ""] = value.value;
    });
    $.ajax({
        url: '/Bill/CreateBill',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(data),
        dataType: 'json',
        success: function (respone) {
            if (respone.status == true) {
                window.location.replace("/Bill/ListBills");
            } else {
                $("#add-message").addClass("text-danger");
                $("#add-message").html(respone.message);
            }
        },
        error: function (respone) {
            console.log(respone);
        }
    });
    return false;
}

//Ajax Hủy đơn hàng
function huyDonHang(id) {
    swal({
        title: "Cảnh báo",
        text: "Bạn có chắc về việc hủy đơn hàng này!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    type: 'POST',
                    data: { "mahd": id, "stt": 0 },
                    url: '/Bill/ChangeStatus',
                    success: function (response) {
                        if (response.status == true) {
                            swal({
                                title: "Thành công!",
                                text: "Hủy đơn hàng thành công !",
                                type: "success",
                                icon: "success",
                                timer: 1500,
                                button: false
                            });
                        } else {
                            swal({
                                title: "Thất bại!",
                                text: "Bạn không thể hủy đơn hàng do đơn hàng đã đang giao",
                                type: "danger",
                                icon: "error",
                                timer: 1500,
                                button: false
                            });
                        }
                        setTimeout(function () {
                            window.location = "/Bill/ListBills";
                        }, 1500);
                    },
                    error: function (response) {
                        //debugger;  
                        console.log(xhr.responseText);
                        alert("Error has occurred..");
                    }
                });
            }
        });
}

