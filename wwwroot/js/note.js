var listObj = {
    perPage: 6,
    page: 1,
    offset: 0,
    total: 0
}

const pagination = createPagination();  //Declare pagination object

var note = {
    init: () => {
        pagination.init("pagination", "paginationPerPageSelect", listObj.total, listObj.perPage, listObj.page, note.onPageChange)
        note.setupDeleteModal();
        note.getNoteList(true);

        $("#filterSearch").on("input", function () {
            listObj.page = 1;
            note.getNoteList(true);
        });

        $("#filterSort").on("change", function () {
            listObj.page = 1;
            note.getNoteList(true);
        });

        $("#filterDateFrom").on("change", function () {
            listObj.page = 1;
            note.getNoteList(true);
        });

        $("#filterDateTo").on("change", function () {
            listObj.page = 1;
            note.getNoteList(true);
        });
    },
    getNoteList: function (isRender = false) {
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
        
        console.log('Filter data being sent:', data);
        $.ajax(
            {
                type: "GET",
                url: "/get-note-list",
                data: data,
                success: function (res) {
                    if (!res) {
                        console.error("No response from server");
                        return;
                    }
                    if (res.success) {
                        const $container = $("#noteListContainer");
                        $container.empty();

                        console.log("Response from server:", res);

                        if (res.notes && res.notes.length > 0) {
                            res.notes.forEach(n => {
                                const card = `
                                <div class="col-12 col-sm-6 col-md-4">
                                    <div class="card h-100 shadow-sm border-2 note-card">
                                        <div class="card-body d-flex flex-column p-4">
                                            <h5 class="card-title mb-2 user-select-none">
                                             ${n.isPinned ? '<i class="bi bi-pin-angle-fill text-danger"></i>' : ''}
                                                <i class="bi bi-sticky text-warning"></i>
                                                ${n.noteTitle}
                                                <small class="text-muted">
                                                    ${n.updatedAt ? '<span class="badge bg-black">edit</span>' : ''}
                                                </small>
                                            </h5>
                                            <p class="card-text flex-grow-1 user-select-none">${n.noteContent}</p>
                                            <div class="d-flex justify-content-between align-items-center mt-3">
                                                <small class="text-muted user-select-none">
                                                    <i class="bi bi-calendar3"></i> ${n.createdAt}
                                                </small>
                                                <div class="d-flex align-items-center">
                                                    <a href="/detail/${n.noteId}" class="btn btn-outline-primary btn-sm me-1" title="Detail">
                                                        <i class="bi bi-eye"></i>
                                                    </a>
                                                    <a href="/edit/${n.noteId}" class="btn btn-outline-primary btn-sm me-1" title="Edit">
                                                        <i class="bi bi-pencil"></i>
                                                    </a>
                                                    <button type="button" class="btn btn-outline-danger btn-sm delete-btn" data-bs-toggle="modal"
                                                    data-bs-target="#deleteModal" data-note-id="${n.noteId}" data-note-title="${n.noteTitle}" title="Delete">
                                                        <i class="bi bi-trash"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>`;
                                $container.append(card);
                            });
                            
                            // Add click event listener for delete buttons
                            $('.delete-btn').off('click').on('click', function() {
                                console.log('Delete button clicked!');
                                console.log('Button data:', $(this).data());
                            });
                        } else {
                            $container.append('<div class="col-12"><p class="text-center">No notes available.</p></div>');
                        }

                        listObj.total = res.total || 0;
                        pagination.totalItems = listObj.total;
                        pagination.currentPage = listObj.page;
                        pagination.rowsPerPage = listObj.perPage;

                        console.log(document.getElementById("noteListContainer"));

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
        console.log('Setting up delete modal:', deleteModal);
        
        if (deleteModal) {
            deleteModal.addEventListener('show.bs.modal', function (event) {
                console.log('Modal is opening...');
                var button = event.relatedTarget;
                var noteId = button.getAttribute('data-note-id');
                var noteTitle = button.getAttribute('data-note-title');
                var form = document.getElementById('deleteForm');
                var message = document.getElementById('deleteModalMessage');

                console.log('Note ID:', noteId, 'Note Title:', noteTitle);

                if (form) {
                    form.action = '/delete/' + noteId;
                    console.log('Form action set to:', form.action);
                }

                if (message) {
                    message.textContent = 'Are you sure you want to delete the note "' + noteTitle + '"?';
                    console.log('Message set:', message.textContent);
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
        note.getNoteList(true);
    }

}