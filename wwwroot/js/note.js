var listObj = {
    perPage: 6,
    page: 1,
    offset: 0,
    total: 0
}

const pagination = createPagination();  //Declare pagination object
let checkDropzone; // Declare 
var myModalAdd; //Declare Global variable

var note = {
    init: () => {
        pagination.init("pagination", "paginationPerPageSelect", listObj.total, listObj.perPage, listObj.page, note.onPageChange)
        note.setupDeleteSA();
        note.applyAdvanceFilter();
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

        $("#openAddNoteModal").on("click", function () {
            note.getModalAddNote();
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
            ToDate: $("#filterDateTo").val(),
            StatusFilter: selectedStatuses.join(',') //join to parse array to string
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
                        $container.html(res.notes);

                        //Bind button for open modal
                        $(document).off('click', '.detail-btn').on('click', '.detail-btn', function () {
                            var noteId = $(this).data('note-id');
                            note.getModalNoteDetail(noteId);
                        });

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
    getModalNoteDetail: function (noteId) {
        $.ajax({
            url: "note-details/" + noteId,
            type: 'GET',
            success: function (res) {
                if (!res) {
                    console.error("No response from server");
                    return;
                }
                if (res.success) {
                    $('#noteModalContainer').empty().html(res.html);

                    myModalAdd = new bootstrap.Modal(document.getElementById('noteDetailModal'));
                    myModalAdd.show();

                    // Bind fancybox for images in the modal
                    if (typeof Fancybox !== 'undefined') {
                        Fancybox.bind('[data-fancybox]', {
                        });
                    }

                    if (!res.isPublic) {
                        if (res.canEdit) {
                            $('#editForm :input').prop('disabled', false);
                            $('#editIsPinnedBox').show();
                            $('#previewFile').hide();
                            $('#editFileAndPublish').show();
                            $('#previewFileModel').show();

                            $('#saveEditBtn').show();

                            //Bind Delete button
                            $(document).off("click", "button[data-action='deleteImageCard']")
                                .on("click", "button[data-action='deleteImageCard']", function () {

                                    const swalWithBootstrapButtons = Swal.mixin({
                                        customClass: {
                                            confirmButton: "btn btn-danger ms-2",
                                            cancelButton: "btn btn-secondary me-2"
                                        },
                                        buttonsStyling: false
                                    });

                                    swalWithBootstrapButtons.fire({
                                        title: "Notification",
                                        text: `คุณแน่ใจหรือไม่ที่จะลบรูปภาพ?`,
                                        icon: "warning",
                                        showCancelButton: true,
                                        confirmButtonText: "ตกลง",
                                        cancelButtonText: "ยกเลิก",
                                        reverseButtons: true
                                        })
                                        .then((result) => {
                                        if (result.isConfirmed) {
                                            var resourceID = $(this).data("img");
                                            note.deleteImage(resourceID);
                                        }
                                    });

                                });
                            //Bind Save Button
                            $('#saveEditBtn').off('click').on('click', function () {
                                note.saveNote(noteId);
                            });

                            note.initDropzone();

                        } else {
                            $('#editForm :input').prop('disabled', true);
                            $('#editIsPinnedBox').hide();
                            $('#previewFile').show();
                            $('#previewFileModel').hide();
                            $('#editFileAndPublish').hide();
                            $('#saveEditBtn').hide();
                        }
                    } else {
                        $('#editForm :input').prop('disabled', true);
                        $('#editIsPinnedBox').hide();
                        $('#previewFile').show();
                        $('#previewFileModel').hide();
                        $('#editFileAndPublish').hide();
                        $('#saveEditBtn').hide();
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
    getModalAddNote: function () {
        $.ajax({
            url: "/get-add-note-modal",
            type: 'GET',
            success: function (res) {
                if (!res) {
                    console.error("No response from server");
                    return;
                }
                if (res.success) {
                    $('#addNoteContainer').empty().html(res.html);
                    
                    note.resetCreateModal();

                    myModalAdd = new bootstrap.Modal(document.getElementById('addNoteModal'));
                    myModalAdd.show();

                    note.initCreateNote();
                    note.initDropzone();
                } else {
                    app.notify("error", res.message);
                }
            },
            error: function () {
                app.notify("error", "ไม่สามารถโหลด modal ได้");
            }
        });
    },
    saveNote: function (noteId) {
        var formData = {
            NoteId: noteId,
            NoteTitle: $('#editNoteTitle').val(),
            NoteContent: $('#editNoteContent').val(),
            IsPinned: $('#editIsPinned').is(':checked'),
            ActiveFrom: $('#publishStartDate').val(),
            ActiveUntil: $('#publishEndDate').val()
        };     
        $.ajax({
            type: "POST",
            url: "/update-note",
            data: formData,
            success: function (res) {
                if (res.success) {
                    // Close modal and refresh list
                    myModalAdd.hide();

                    note.getNoteList(false);
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
    showCreateErrors: function (errors) {
        var errorHtml = '<ul class="mb-0">';
        errors.forEach(function (error) {
            errorHtml += '<li>' + error + '</li>';
        });
        errorHtml += '</ul>';

        $('#createErrors').html(errorHtml).show();
    },
    setupDeleteSA: function () {
        // Add click event listener for delete buttons
        $(document).on('click', '.delete-btn', function (e) {
            e.preventDefault();

            var noteId = $(this).data('note-id');
            var noteTitle = $(this).data('note-title');
            var canDelete = $(this).data('can-delete');

            if (!canDelete) {
                Swal.fire({
                    title: "Notification",
                    text: `คุณไม่มีสิทธิ์ในการลบ "${noteTitle}"`,
                    icon: "error",
                    confirmButtonText: "ตกลง"
                });
                return;
            }

            const swalWithBootstrapButtons = Swal.mixin({
                customClass: {
                    confirmButton: "btn btn-danger ms-2",
                    cancelButton: "btn btn-secondary me-2"
                },
                buttonsStyling: false
            });

            swalWithBootstrapButtons.fire({
                title: "Notification",
                text: `คุณแน่ใจหรือไม่ที่จะลบ "${noteTitle}"?`,
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
                        url: "/delete/" + noteId,
                        success: function (res) {
                            swalWithBootstrapButtons.fire({
                                title: "Notification",
                                text: `"${noteTitle}" ถูกลบเรียบร้อยแล้ว`,
                                icon: "success"
                            });
                            // Refresh the note list
                            note.getNoteList(false);
                        },
                        error: function () {
                            swalWithBootstrapButtons.fire({
                                title: "Notification",
                                text: `ไม่สามารถลบ ${noteTitle} ได้`,
                                icon: "error"
                            });
                        }
                    });
                }
            });
        });
    },
    resetCreateModal: function () {
        $('#createErrors').hide().empty();
        $('#createNoteTitle').val('');
        $('#createNoteContent').val('');
        $('#publishStartDate').val('');
        $('#publishEndDate').val('');
        $('#createIsPinned').prop('checked', false);

        // Clear file input
        var fileInput = document.getElementById('cameraInput');
        if (fileInput) {
            fileInput.value = '';
        }

        if (typeof Dropzone !== 'undefined' && Dropzone.instances.length > 0) {
            Dropzone.instances.forEach(function (dz) {
                if (dz.element.id === 'previewDropzone') {
                    dz.removeAllFiles(true);
                    dz.destroy();
                }
            });
        }

        $('#saveCreateBtn').off('click');
        $('#cancelCreateBtn').off('click');
    },
    initCreateNote: function () {
        $('#saveCreateBtn').off('click').on('click', function () {
            const files = checkDropzone.getAcceptedFiles();

            const formData = new FormData();
            formData.append('NoteTitle', $('#createNoteTitle').val());
            formData.append('NoteContent', $('#createNoteContent').val());
            formData.append('IsPinned', $('#createIsPinned').is(':checked'));
            
            var startDate = $('#createPublishStartDate').val() || new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 16);
            //สูตรแปลง UTC เป็น local ถ้าไม่แปลงมันจะเป็น UTC ซึ่งในโปนเจคไม่ใช้
            //if null set to for date time now local format
            formData.append('ActiveFrom', startDate);

            formData.append('ActiveUntil', $('#createPublishEndDate').val());
            // Get files from dropzone
            files.forEach((file, index) => {
                formData.append('files', file, file.name);
            });
            note.createNote(formData);
        });

        $('#cancelCreateBtn').off('click').on('click', function () {
            if (myModalAdd) {
                myModalAdd.hide();
            }
        });
    },
    createNote: function (data) {
        $.ajax({
            type: "POST",
            url: "/create-note",
            data: data,
            processData: false,
            contentType: false,
            success: function (res) {
                if (res.success) {
                    // Close modal and refresh list
                    if (myModalAdd) {
                        myModalAdd.hide();
                    }
                    note.getNoteList(false);

                    // Show success message (optional)
                    //note.showNotification('success', 'Note created successfully!');
                } else {
                    note.showCreateErrors(res.errors);
                }
            },
            error: function () {
                note.showCreateErrors(['An error occurred while saving.']);
            }
        });
    },
    onPageChange(currentPage, rowsPerPage, offset) {
        listObj.page = currentPage;
        listObj.perPage = rowsPerPage;
        listObj.offset = (currentPage - 1) * rowsPerPage;
        note.getNoteList(true);
    },
    applyAdvanceFilter: function () {
        $('#applyAdvanceFilter').unbind('click').on('click', function () {
            $('#advanceFilterModal').modal('hide');
            listObj.page = 1;
            note.getNoteList(true);
        });
    },
    //Dropzone script from PTAR
    initDropzone: function () {
        Dropzone.autoDiscover = false;

        // เคลียร์ Dropzone ก่อน - ทำลายเฉพาะ instance ที่เกี่ยวข้องกับ previewDropzone
        if (Dropzone.instances.length > 0) {
            Dropzone.instances.forEach(function (dz) {
                if (dz.element.id === 'previewDropzone') {
                    dz.destroy();
                }
            });
        }

        checkDropzone = new Dropzone("#previewDropzone", {
            url: "/create-note",
            autoProcessQueue: false,
            acceptedFiles: "image/*,video/*,application/pdf",
            addRemoveLinks: true,
            dictRemoveFile: "",
            clickable: true,
            init: function () {
                const dzInstance = this;
                const inputEl = document.getElementById("cameraInput");
                const dt = new DataTransfer(); // สำหรับจำลองไฟล์ input

                // เมื่อเลือกไฟล์จาก input ของเราเอง
                inputEl.addEventListener("change", function () {
                    for (let i = 0; i < inputEl.files.length; i++) {
                        const file = inputEl.files[i];
                        dzInstance.addFile(file);     // เพิ่มไฟล์ลง Dropzone
                        dt.items.add(file);          // เก็บไฟล์ลง DataTransfer
                    }
                    inputEl.files = dt.files;         // อัปเดต input
                });


                // เมื่อ Dropzone เพิ่มไฟล์
                dzInstance.on("addedfile", function (file) {
                    const preview = file.previewElement.querySelector(".dz-image");
                    if (!preview) return;

                    // ถ้าเป็น video แทน preview รูป
                    if (file.type.startsWith("video/")) {
                        const video = document.createElement("video");
                        video.controls = true;
                        video.src = URL.createObjectURL(file);
                        video.style.width = "140px";
                        video.style.height = "140px";
                        video.style.borderRadius = "8px";
                        video.style.objectFit = "cover";

                        preview.innerHTML = "";
                        preview.appendChild(video);
                    }

                    // ปรับปุ่มลบ
                    const removeLink = file.previewElement.querySelector(".dz-remove");
                    if (removeLink) {
                        removeLink.innerHTML = '<i class="fas fa-times"></i>';
                        removeLink.title = "Remove file";
                        removeLink.style.fontSize = "18px";
                    }
                });

                // เมื่อ Dropzone ลบไฟล์
                dzInstance.on("removedfile", function (file) {
                    // ลบไฟล์จาก DataTransfer (input ของเรา)
                    for (let i = 0; i < dt.items.length; i++)
                    {
                        if (dt.items[i].getAsFile() === file) {
                            dt.items.remove(i);
                            break;
                        }
                    }
                    inputEl.files = dt.files; // อัปเดต input ให้ตรงกับ Dropzone
                });
            }
        });
    },
    //Delete Image from dropzone PTAR
    initDeleteImage: function () {
        $('')
    },
    deleteImage: function (resourceID) {
        $.ajax({
            url: "/card-deleteimage",
            type: 'POST',
            data: { resourceID: resourceID },
            success: function (res) {
                if (res.success) {
                    // ลบกล่อง preview-item ออก
                    $('.preview-item[data-img="' + resourceID + '"]').remove();
                } else {
                    app.notify("error", res.message);
                }
            },
            error: function () {
            }
        });
    }
}
