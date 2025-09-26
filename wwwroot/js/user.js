var listObj = {
    perPage: 6,
    page: 1,
    offset: 0,
    total: 0
}

const pagination = createPagination();  //Declare pagination object

var user = {
    init: () => {
        pagination.init("pagination", "paginationPerPageSelect", listObj.total, listObj.perPage, listObj.page, user.onPageChange)
        user.setupDeleteModal();
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
                                        `<button type="button"
                                                class="btn btn-outline-danger btn-sm delete-btn"
                                                data-bs-toggle="modal"
                                                data-bs-target="#deleteModal" 
                                                data-user-id="${n.userId}" 
                                                data-user-title="${n.email}" 
                                                title="Delete">
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
    setupDeleteModal: function() {
        var deleteModal = document.getElementById('deleteModal');
        
        if (deleteModal) {
            deleteModal.addEventListener('show.bs.modal', function (event) {
                var button = event.relatedTarget;
                var userId = button.getAttribute('data-user-id');
                var email = button.getAttribute('data-user-title');
                var form = document.getElementById('deleteForm');
                var message = document.getElementById('deleteModalMessage');

                if (form) {
                    form.action = 'user-manager/delete/' + userId;
                    //console.log('Form action set to:', form.action);
                }

                if (message) {
                    message.textContent = 'Are you sure you want to delete User "' + email + '"?';
                    //console.log('Message set:', message.textContent);
                }
            });
        } else {
            console.error('Delete modal element not found!');
        }
    },
    onPageChange(currentPage, rowsPerPage, offset) {
        listObj.page = currentPage;
        listObj.perPage = rowsPerPage;
        listObj.offset = (currentPage - 1) * rowsPerPage;
        user.getUserList(true);
    }

}