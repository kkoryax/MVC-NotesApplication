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
        note.setupDetailModal();
        note.setupEditMode();
        note.setupCreateModal();
        note.setupDeleteModal();
        note.setupPermissionDeniedModal();
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

                        if (res.notes && res.notes.length > 0) {
                            res.notes.forEach(n => {
                                const card = `
                                <div class="col-12 col-sm-6 col-md-6 col-xl-4">
                                    <div class="card h-150 shadow-sm border-2 note-card">
                                        <div class="card-body d-flex flex-column p-4">
                                            <h5 class="card-title mb-2 user-select-none text-truncate">
                                             ${n.isPinned ? '<i class="bi bi-pin-angle-fill text-danger"></i>' : ''}
                                                <i class="bi bi-sticky text-warning"></i>
                                                ${n.noteTitle}
                                                <small class="text-muted">
                                                    ${n.updatedAt ? '<span class="badge bg-black">edit</span>' : ''}
                                                </small>
                                            </h5>
                                            <p class="card-text user-select-none" style="display: overflow: hidden; line-height: 1.4; min-height: 4em;">${n.noteContent}</p>
                                            <div class="mt-auto">
                                                <div class="d-flex justify-content-between align-items-center">
                                                    <div class="d-flex flex-column">
                                                        <small class="text-muted user-select-none text-truncate">
                                                            <i class="bi bi-calendar3"></i> ${n.createdAt}
                                                        </small>
                                                        <small class="text-muted user-select-none text-truncate">
                                                            <i class="bi bi-person"></i> ${n.createdByUserEmail}
                                                        </small>
                                                    </div>
                                                    <div class="d-flex gap-1">
                                                        <button type="button" class="btn btn-outline-primary btn-sm detail-btn"
                                                                data-bs-toggle="modal" data-bs-target="#noteDetailModal"
                                                                data-note-id="${n.noteId}" title="View Detail">
                                                            <i class="bi bi-eye"></i>
                                                        </button>
                                                        ${n.canDelete ? `
                                                        <button type="button" class="btn btn-outline-danger btn-sm delete-btn" data-bs-toggle="modal"
                                                            data-bs-target="#deleteModal" data-note-id="${n.noteId}" data-note-title="${n.noteTitle}" title="Delete">
                                                                <i class="bi bi-trash"></i>
                                                        </button>
                                                        ` : `
                                                        <button type="button" class="btn btn-outline-danger btn-sm delete-btn" data-bs-toggle="modal"
                                                            data-bs-target="#permissionDeniedModal" data-note-id="${n.noteId}" data-note-title="${n.noteTitle}" title="Delete">
                                                                <i class="bi bi-trash"></i>
                                                        </button>
                                                        `}
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>`;
                                $container.append(card);
                            });
                            
                            // Add click event listener for delete buttons
                            $('.delete-btn').off('click').on('click', function() {
                                //console.log('Delete button clicked!');
                                //console.log('Button data:', $(this).data());
                            });
                        } else {
                            $container.append('<div class="col-12"><p class="text-center">No notes available.</p></div>');
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
    setupDetailModal: function () {
        var detailModal = document.getElementById('noteDetailModal');

        if (detailModal) {
            detailModal.addEventListener('show.bs.modal', function (event) {
                var button = event.relatedTarget;
                var noteId = button.getAttribute('data-note-id');

                note.loadNoteDetail(noteId);
            });

            // Reset modal when hidden
            detailModal.addEventListener('hidden.bs.modal', function () {
                note.resetModal();
            });
        }
    },
    resetModal: function () {
        // Clear form and errors; keep edit mode as the only mode
        $('#editErrors').hide().empty();
        $('#editNoteId').val('');
        $('#editNoteTitle').val('');
        $('#editNoteContent').val('');
        $('#editIsPinned').prop('checked', false);
        // Ensure inputs are enabled and Save visible by default
        $('#editForm :input').prop('disabled', false);
        $('#saveEditBtn').show();
    },
    loadNoteDetail: function (noteId) {
        $.ajax({
            type: "GET",
            url: "/note-details/" + noteId,
            success: function (res) {
                if (res.success) {
                    var note = res.note;

                    // Header info
                    $('#modalNoteTitle').text(note.noteTitle);
                    $('#modalCreatedAt').text('Created: ' + note.createdAt);
                    $('#modalCreatedBy').text('Created by ' + note.createdByUserEmail);

                    // Populate edit form directly
                    $('#editNoteId').val(note.noteId);
                    $('#editNoteTitle').val(note.noteTitle);
                    $('#editNoteContent').val(note.noteContent);
                    $('#editIsPinned').prop('checked', !!note.isPinned);

                    // Permissions: disable form and hide Save when cannot edit
                    if (note.canEdit) {
                        $('#editForm :input').prop('disabled', false);
                        $('#saveEditBtn').show();
                        //$('#cancelEditBtn').show();
                        $('#editIsPinnedBox').show();
                    } else {
                        $('#editForm :input').prop('disabled', true);
                        $('#saveEditBtn').hide();
                        //$('#cancelEditBtn').hide();
                        $('#editIsPinnedBox').hide();
                    }

                    if (note.updatedAt) {
                        $('#modalUpdatedAt').text('Updated: ' + note.updatedAt).show();
                    }
                    if (note.isPinned) {
                        $('#modalNotePinned').show();
                    } else {
                        $('#modalNotePinned').hide();
                    }
       
                } else {
                    console.error('Failed to load note detail:', res.message);
                }
            },
            error: function () {
                console.error('Error loading note detail');
            }
        });
    },
    setupEditMode: function () {
        $('#cancelEditBtn').on('click', function () {
            var modalEl = document.getElementById('noteDetailModal');
            if (modalEl) {
                var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                modal.hide();
            }
        });

        $('#saveEditBtn').on('click', function () {
            note.saveNote();
        });
    },

    saveNote: function () {
        var formData = {
            NoteId: $('#editNoteId').val(),
            NoteTitle: $('#editNoteTitle').val(),
            NoteContent: $('#editNoteContent').val(),
            IsPinned: $('#editIsPinned').is(':checked')
        };

        $.ajax({
            type: "POST",
            url: "/update-note",
            data: formData,
            success: function (res) {
                if (res.success) {
                    // Update modal data and refresh list so pin icons reflect
                    note.loadNoteDetail(formData.NoteId);
                    note.getNoteList(false);

                    // Show success message (optional)
                    //note.showNotification('success', 'Note updated successfully!');
                } else {
                    note.showEditErrors(res.errors);
                }
            },
            error: function () {
                note.showEditErrors(['An error occurred while saving.']);
            }
        });
    },

    showEditErrors: function (errors) {
        var errorHtml = '<ul class="mb-0">';
        errors.forEach(function (error) {
            errorHtml += '<li>' + error + '</li>';
        });
        errorHtml += '</ul>';

        $('#editErrors').html(errorHtml).show();
    },
    setupDeleteModal: function() {
        var deleteModal = document.getElementById('deleteModal');
        
        if (deleteModal) {
            deleteModal.addEventListener('show.bs.modal', function (event) {
                var button = event.relatedTarget;
                var noteId = button.getAttribute('data-note-id');
                var noteTitle = button.getAttribute('data-note-title');
                var form = document.getElementById('deleteForm');
                var message = document.getElementById('deleteModalMessage');

                if (form) {
                    form.action = '/delete/' + noteId;
                    //console.log('Form action set to:', form.action);
                }

                if (message) {
                    message.textContent = 'Are you sure you want to delete this note "' + noteTitle + '"?';
                    //console.log('Message set:', message.textContent);
                }
            });
        } else {
            console.error('Delete modal element not found!');
        }
    },
    setupCreateModal: function () {
        var createModal = document.getElementById('createModal');

        if (createModal) {
            createModal.addEventListener('hidden.bs.modal', function () {
                note.resetCreateModal();
            });
        }

        $('#cancelCreateBtn').on('click', function () {
            var modalEl = document.getElementById('createModal');
            if (modalEl) {
                var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                modal.hide();
            }
        });

        $('#saveCreateBtn').on('click', function () {
            note.createNote();
        });
    },
    resetCreateModal: function () {
        // Clear form and errors
        $('#createErrors').hide().empty();
        $('#createNoteTitle').val('');
        $('#createNoteContent').val('');
        $('#createIsPinned').prop('checked', false);
    },
    createNote: function () {
        var formData = {
            NoteTitle: $('#createNoteTitle').val(),
            NoteContent: $('#createNoteContent').val(),
            IsPinned: $('#createIsPinned').is(':checked')
        };

        $.ajax({
            type: "POST",
            url: "/create-note",
            data: formData,
            success: function (res) {
                if (res.success) {
                    // Close modal and refresh list
                    var modalEl = document.getElementById('createModal');
                    if (modalEl) {
                        var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                        modal.hide();
                    }
                    note.getNoteList(false);
                    
                    // Show success message (optional)
                    //note.showNotification('success', 'Note created successfully!');
                } else {
                    note.showCreateErrors(res.errors);
                }
            },
            error: function () {
                note.showCreateErrors(['เกิดปัญหาขัดข้อง.']);
            }
        });
    },
    showCreateErrors: function (errors) {
        var errorHtml = '<ul class="mb-0">';
        errors.forEach(function (error) {
            errorHtml += '<li>' + error + '</li>';
        });
        errorHtml += '</ul>';

        $('#createErrors').html(errorHtml).show();
    },
    setupPermissionDeniedModal: function () {
        var permissionDeniedModal = document.getElementById('permissionDeniedModal');

        if (permissionDeniedModal) {
            permissionDeniedModal.addEventListener('show.bs.modal', function (event) {
                var button = event.relatedTarget;
                var noteTitle = button ? button.getAttribute('data-note-title') : '';
                var message = document.getElementById('permissionDeniedModalMessage');

                if (message) {
                    message.textContent = 'You not have permission to delete this note "' + noteTitle + '".';
                }
            });
        } else {
            console.error('Permission denied modal element not found!');
        }
    },
    onPageChange(currentPage, rowsPerPage, offset) {
        listObj.page = currentPage;
        listObj.perPage = rowsPerPage;
        listObj.offset = (currentPage - 1) * rowsPerPage;
        note.getNoteList(true);
    }
    //Edit Button
    //< a href = "/edit/${n.noteId}" class="btn btn-outline-primary btn-sm me-1" title = "Edit" >
    //  <i class="bi bi-pencil"></i>
    //</a >

}