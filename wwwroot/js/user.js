var listObj = {
    perPage: 10,
    page: 1,
    offset: 0,
    total: 0
}

const pagination = createPagination();  //Declare pagination object

var user = {
    init: () => {
        pagination.init("pagination", "paginationPerPageSelect", listObj.total, listObj.perPage, listObj.page, user.onPageChange)
        user.setupDeleteSA();
        user.getUserList(true);

        $("#filterSearch").on("input", function () {
            listObj.page = 1;
            user.getUserList(true);
        });

        $("#filterSort").on("change", function () {
            listObj.page = 1;
            user.getUserList(true);
        });

        $("#filterDateFrom").on("change", function () {
            listObj.page = 1;
            user.getUserList(true);
        });

        $("#filterDateTo").on("change", function () {
            listObj.page = 1;
            user.getUserList(true);
        });
    },
    getUserList: function (isRender = false) {
        var data =
        {
            PerPage: listObj.perPage,
            Page: listObj.page,
            Offset: listObj.offset,
            Total: listObj.total,
            Search: $("#filterSearch").val(),
            Sort: $("#filterSort").val(),
            FromDate: $("#filterDateFrom").val(),
            ToDate: $("#filterDateTo").val()
        }
        
        $.ajax(
            {
                type: "GET",
                url: "/get-user-list",
                data: data,
                success: function (res) {
                    if (!res) {
                        console.error("No response from server");
                        return;
                    }
                    if (res.success) {
                        const $container = $("#userListContainer");
                        $container.empty();

                        if (res.users && res.users.length > 0) {
                            res.users.forEach(n => {
                                const card = `
                                <tr>
                                    <td>${n.email}</td>
                                    <td>${n.role}</td>
                                    <td>${n.createdAt}</td>
                                    <td>
                                       ${
                                            n.role === "User" ?
                                    `<button type="button" class="btn btn-outline-danger btn-sm delete-btn" 
                                    data-user-id="${n.userId}" data-user-email="${n.email}" title="Delete">
                                         <i class="bi bi-trash"></i>
                                    </button>`
                                            : ""
                                        }
                                    </td>
                                </tr>
                                `;
                                $container.append(card);
                            });
                            
                            // Add click event listener for delete buttons
                            $('.delete-btn').off('click').on('click', function() {
                                //console.log('Delete button clicked!');
                                //console.log('Button data:', $(this).data());
                            });
                        } else {
                            $container.append('<div class="col-12"><p class="text-center">No User in database</p></div>');
                        }

                        listObj.total = res.total || 0;
                        pagination.totalItems = listObj.total;
                        pagination.currentPage = listObj.page;
                        pagination.rowsPerPage = listObj.perPage;

                        if (isRender) {
                            pagination.render();
                        }
                    } else {
                        app.notify("error", res.message);
                    }
                },
                error: function () {
                    window.location.reload();
                }
            });
    },
    setupDeleteSA: function () {
        // Add click event listener for delete buttons
        $(document).on('click', '.delete-btn', function (e) {
            e.preventDefault();

            var userId = $(this).data('user-id');
            var user_email = $(this).data('user-email');

            const swalWithBootstrapButtons = Swal.mixin({
                customClass: {
                    confirmButton: "btn btn-danger ms-2",
                    cancelButton: "btn btn-secondary me-2"
                },
                buttonsStyling: false
            });

            swalWithBootstrapButtons.fire({
                title: "Notification",
                text: `คุณแน่ใจหรือไม่ที่จะลบ "${user_email}"?`,
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "ตกลง",
                cancelButtonText: "ยกเลิก",
                reverseButtons: true
            }).then((result) => {
                if (result.isConfirmed) {
                    // Send delete request
                    $.ajax({
                        type: "POST",
                        url: "user-manager/delete/" + userId,
                        success: function (res) {
                            swalWithBootstrapButtons.fire({
                                title: "Notification",
                                text: `"${user_email}" ถูกลบเรียบร้อยแล้ว`,
                                icon: "success"
                            });
                            // Refresh the note list
                            user.getUserList(false);
                        },
                        error: function () {
                            swalWithBootstrapButtons.fire({
                                title: "Notification",
                                text: `ไม่สามารถลบ ${user_email} ได้`,
                                icon: "error"
                            });
                        }
                    });
                }
            });
        });
    },
    onPageChange(currentPage, rowsPerPage, offset) {
        listObj.page = currentPage;
        listObj.perPage = rowsPerPage;
        listObj.offset = (currentPage - 1) * rowsPerPage;
        user.getUserList(true);
    }

}