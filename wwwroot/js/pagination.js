//Pagination JS template from P'TAR project
function createPagination() {

    return {

        currentPage: 1,

        totalItems: 0,

        rowsPerPage: 10,

        maxVisiblePages: 3,

        containerId: "pagination",

        rowsPerPageSelectId: "paginationPerPageSelect",

        onPageChanged: null,

        get totalPages() {

            return Math.ceil(this.totalItems / this.rowsPerPage);

        },

        init(containerId = "pagination", rowsPerPageSelectId = "paginationPerPageSelect", totalItems = 0, rowsPerPage = 10, currentPage = 1, onPageChanged = null) {

            this.containerId = containerId;

            this.rowsPerPageSelectId = rowsPerPageSelectId;

            this.totalItems = totalItems;

            this.rowsPerPage = rowsPerPage;

            this.currentPage = currentPage;

            this.onPageChanged = onPageChanged;

            this.render();

            this.registerEvents();

        },

        goToPage(page) {

            if (page < 1 || page > this.totalPages)

                return;

            this.currentPage = page;

            if (typeof this.onPageChanged === "function") {
                const offset = (this.currentPage - 1) * this.rowsPerPage;

                this.onPageChanged(this.currentPage, this.rowsPerPage, offset);

            }

            this.render();

        },

        createPageItem(page, isActive = false, isDisabled = false, isEllipsis = false) {

            if (isEllipsis) {

                return `<li class="page-item disabled"><span class="page-link">...</span></li>`;

            }

            return `
                <li class="page-item ${isActive ? 'active' : ''} ${isDisabled ? 'disabled' : ''}">
                <a class="page-link" href="#" data-page="${page}">${page}</a>
                </li>`;

        },

        registerEvents() {

            const container = document.getElementById(this.containerId);

            container.addEventListener("click", (e) => {

                const target = e.target.closest('a[data-page]');

                if (target) {

                    e.preventDefault();

                    const page = parseInt(target.dataset.page);

                    this.goToPage(page, true);

                }

            });

            const rowsSelect = document.getElementById(this.rowsPerPageSelectId);

            if (rowsSelect) {

                rowsSelect.addEventListener("change", (e) => {

                    this.rowsPerPage = parseInt(e.target.value);

                    this.currentPage = 1;

                    if (typeof this.onPageChanged === "function") {

                        const offset = (this.currentPage - 1) * this.rowsPerPage;

                        this.onPageChanged(this.currentPage, this.rowsPerPage, offset);

                    }

                    this.render();

                });

            }

        },

        render() {
            const container = document.getElementById(this.containerId);

            container.innerHTML = "";

            const { currentPage, totalPages, maxVisiblePages } = this;

            // First / Prev

            container.innerHTML += `
                <li class="page-item ${currentPage === 1 || this.totalItems === 0 ? 'disabled' : ''}">
                <a class="page-link" href="#" data-page="1">
                <i class="tf-icon bx bx-chevrons-left bx-sm"></i>
                </a>
                </li>
                <li class="page-item ${currentPage === 1 || this.totalItems === 0 ? 'disabled' : ''}">
                <a class="page-link" href="#" data-page="${currentPage - 1}">
                <i class="tf-icon bx bx-chevron-left bx-sm"></i>
                </a>
                </li>`;

            let startPage, endPage;

            if (totalPages <= maxVisiblePages + 2) {

                startPage = 1;

                endPage = totalPages;

            } else {

                const halfRange = Math.floor(maxVisiblePages / 2);

                if (currentPage <= 2) {

                    startPage = 1;

                    endPage = maxVisiblePages;

                } else if (currentPage >= totalPages - 1) {

                    startPage = totalPages - maxVisiblePages + 1;

                    endPage = totalPages;

                } else {

                    startPage = currentPage - halfRange;

                    endPage = currentPage + halfRange;

                }

            }

            if (startPage > 1) {

                container.innerHTML += this.createPageItem(1);

                if (startPage > 2) {

                    container.innerHTML += this.createPageItem(null, false, false, true);

                }

            }

            for (let i = startPage; i <= endPage; i++) {

                container.innerHTML += this.createPageItem(i, i === currentPage);

            }

            if (endPage < totalPages) {

                if (endPage < totalPages - 1) {

                    container.innerHTML += this.createPageItem(null, false, false, true);

                }

                container.innerHTML += this.createPageItem(totalPages);

            }

            // Next / Last
            container.innerHTML += `
                <li class="page-item ${currentPage === totalPages || this.totalItems === 0 ? 'disabled' : ''}">
                <a class="page-link" href="#" data-page="${currentPage + 1}">
                <i class="tf-icon bx bx-chevron-right bx-sm"></i>
                </a>
                </li>
                <li class="page-item ${currentPage === totalPages || this.totalItems === 0 ? 'disabled' : ''}">
                <a class="page-link" href="#" data-page="${totalPages}">
                <i class="tf-icon bx bx-chevrons-right bx-sm"></i>
                </a>
                </li>`;

            // Summary text
            const startItem = (currentPage - 1) * this.rowsPerPage + 1;

            const endItem = Math.min(currentPage * this.rowsPerPage, this.totalItems);

        }

    }

};
