/**
 * Modul Controller Javascript untuk Master Type (Vanilla JS & Tailwind)
 */
const typeController = (function () {
    let state = {
        keyword: '',
        page: 1,
        limit: 10
    };

    // DOM Elements
    let tbody, form, modalEl, searchInput, paginationContainer, brandDropdown;

    function init() {
        tbody = document.getElementById('typeTableBody');
        form = document.getElementById('typeForm');
        modalEl = document.getElementById('typeModal');
        searchInput = document.getElementById('searchInput');
        paginationContainer = document.getElementById('paginationContainer');
        brandDropdown = document.getElementById('typeBrandId');

        loadBrands(); // Panggil data Brand untuk mengisi Dropdown
        loadData();

        searchInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') search();
        });
    }

    async function webCall(url, method = 'GET', data = null) {
        const options = {
            method: method,
            headers: {
                'Content-Type': 'application/json'
            }
        };

        if (data) {
            options.body = JSON.stringify(data);
        }

        const response = await fetch(url, options);

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const result = await response.json();

        if (result.status === "Error") {
            throw new Error(result.message);
        }

        return result;
    }

    // Mengambil data Brand untuk diletakkan di Select Option
    async function loadBrands() {
        try {
            // Ambil data brand dengan limit besar agar semua brand terambil
            const response = await webCall('/Brand/List?page=1&limit=1000');
            const brands = response.data;

            brandDropdown.innerHTML = '<option value="">-- Pilih Brand --</option>';
            brands.forEach(b => {
                const option = document.createElement('option');
                option.value = b.id;
                option.textContent = `${b.code} - ${b.name}`;
                brandDropdown.appendChild(option);
            });
        } catch (error) {
            console.error("Gagal memuat list brand:", error);
        }
    }

    // Panggil Endpoint: /Type/List
    async function loadData() {
        tbody.innerHTML = '<tr><td colspan="6" class="px-6 py-8 text-center text-sm text-gray-500">Memuat data...</td></tr>';

        // Endpoint menggunakan Parameter 'keyword' sesuai controller
        const queryUrl = `/Type/List?keyword=${encodeURIComponent(state.keyword)}&page=${state.page}&limit=${state.limit}`;

        try {
            const response = await webCall(queryUrl);
            renderTable(response.data);
            renderPagination(response.pagination);
        } catch (error) {
            console.error("Gagal memuat data:", error);
            tbody.innerHTML = '<tr><td colspan="6" class="px-6 py-8 text-center text-sm text-red-500">Terjadi kesalahan saat memuat data.</td></tr>';
            paginationContainer.classList.add('hidden');
        }
    }

    function renderTable(data) {
        tbody.innerHTML = '';

        if (!data || data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" class="px-6 py-8 text-center text-sm text-gray-500">Tidak ada data ditemukan.</td></tr>';
            return;
        }

        data.forEach((item) => {
            // Logika label status
            const statusBadge = item.isActive
                ? `<span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">Aktif</span>`
                : `<span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800">Nonaktif</span>`;

            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td class="px-6 py-4 whitespace-nowrap">
                    <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800 uppercase">
                        ${item.code}
                    </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 font-medium">${item.name}</td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">${item.brandName}</td>
                <td class="px-6 py-4 whitespace-nowrap text-center text-sm text-gray-500">${item.year}</td>
                <td class="px-6 py-4 whitespace-nowrap text-center">${statusBadge}</td>
                <td class="px-6 py-4 whitespace-nowrap text-center text-sm font-medium">
                    <button onclick="typeController.showEditModal(${item.id})" class="text-indigo-600 hover:text-indigo-900 bg-indigo-50 hover:bg-indigo-100 px-3 py-1 rounded-md mr-2 transition-colors">Edit</button>
                    <button onclick="typeController.deleteData(${item.id})" class="text-red-600 hover:text-red-900 bg-red-50 hover:bg-red-100 px-3 py-1 rounded-md transition-colors">Hapus</button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    }

    function renderPagination(pagination) {
        const totalPages = pagination?.totalPages || pagination?.TotalPages || 0;
        const currentPage = pagination?.currentPage || pagination?.CurrentPage || state.page;
        const totalItems = pagination?.totalItems || pagination?.TotalItems || 0;

        if (totalPages <= 1) {
            paginationContainer.innerHTML = '';
            paginationContainer.classList.add('hidden');
            return;
        }

        paginationContainer.classList.remove('hidden');

        const prevDisabled = currentPage === 1 ? 'disabled class="opacity-50 cursor-not-allowed"' : `onclick="typeController.changePage(${currentPage - 1})" class="hover:bg-gray-50 cursor-pointer"`;
        const nextDisabled = currentPage === totalPages ? 'disabled class="opacity-50 cursor-not-allowed"' : `onclick="typeController.changePage(${currentPage + 1})" class="hover:bg-gray-50 cursor-pointer"`;

        let pageButtons = '';
        for (let i = 1; i <= totalPages; i++) {
            if (i === currentPage) {
                pageButtons += `<button aria-current="page" class="relative z-10 inline-flex items-center bg-indigo-600 px-4 py-2 text-sm font-semibold text-white focus:z-20 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600">${i}</button>`;
            } else {
                pageButtons += `<button onclick="typeController.changePage(${i})" class="relative inline-flex items-center px-4 py-2 text-sm font-semibold text-gray-900 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0">${i}</button>`;
            }
        }

        const startItem = ((currentPage - 1) * state.limit) + 1;
        const endItem = Math.min(currentPage * state.limit, totalItems);

        paginationContainer.innerHTML = `
            <div class="flex items-center justify-between w-full">
                <div class="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
                    <div>
                        <p class="text-sm text-gray-700">
                            Menampilkan <span class="font-medium">${startItem}</span> - <span class="font-medium">${endItem}</span> dari <span class="font-medium">${totalItems}</span> hasil
                        </p>
                    </div>
                    <div>
                        <nav class="isolate inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
                            <button ${prevDisabled} class="relative inline-flex items-center rounded-l-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 focus:z-20 focus:outline-offset-0">
                                <span class="sr-only">Previous</span>
                                <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true"><path fill-rule="evenodd" d="M12.79 5.23a.75.75 0 01-.02 1.06L8.832 10l3.938 3.71a.75.75 0 11-1.04 1.08l-4.5-4.25a.75.75 0 010-1.08l4.5-4.25a.75.75 0 011.06.02z" clip-rule="evenodd" /></svg>
                            </button>
                            ${pageButtons}
                            <button ${nextDisabled} class="relative inline-flex items-center rounded-r-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 focus:z-20 focus:outline-offset-0">
                                <span class="sr-only">Next</span>
                                <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true"><path fill-rule="evenodd" d="M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z" clip-rule="evenodd" /></svg>
                            </button>
                        </nav>
                    </div>
                </div>
            </div>
        `;
    }

    function changePage(page) {
        state.page = page;
        loadData();
    }

    function search() {
        state.keyword = searchInput.value;
        state.page = 1;
        loadData();
    }

    function showModal() { modalEl.classList.remove('hidden'); }
    function hideModal() { modalEl.classList.add('hidden'); }

    function showAddModal() {
        form.reset();
        document.getElementById('typeId').value = '';
        document.getElementById('typeIsActive').checked = true; // Default aktif saat create
        document.getElementById('typeModalLabel').innerText = 'Tambah Tipe Baru';
        showModal();
    }

    // Panggil Endpoint: /Type/Detail?id={id}
    async function showEditModal(id) {
        try {
            const response = await webCall(`/Type/Detail?id=${id}`);
            const data = response.data;

            document.getElementById('typeId').value = data.id;
            document.getElementById('typeBrandId').value = data.brandId;
            document.getElementById('typeCode').value = data.code;
            document.getElementById('typeName').value = data.name;
            document.getElementById('typeYear').value = data.year;
            document.getElementById('typeIsActive').checked = data.isActive;

            document.getElementById('typeModalLabel').innerText = 'Edit Tipe';
            showModal();
        } catch (error) {
            alert(`Gagal mengambil data: ${error.message}`);
        }
    }

    // Panggil Endpoint: /Type/Create atau /Type/Update?id={id}
    async function save() {
        if (!form.checkValidity()) {
            form.reportValidity();
            return;
        }

        const id = document.getElementById('typeId').value;
        const isEdit = id !== '';

        // Membentuk payload sesuai dengan TypeRequest di Controller
        const payload = {
            brandId: parseInt(document.getElementById('typeBrandId').value),
            code: document.getElementById('typeCode').value.toUpperCase(),
            name: document.getElementById('typeName').value,
            year: parseInt(document.getElementById('typeYear').value),
            isActive: document.getElementById('typeIsActive').checked
        };

        const method = isEdit ? 'PUT' : 'POST';
        const url = isEdit ? `/Type/Update?id=${id}` : `/Type/Create`;

        try {
            await webCall(url, method, payload);
            hideModal();
            alert(`Data berhasil ${isEdit ? 'diperbarui' : 'disimpan'}.`);
            loadData();
        } catch (error) {
            console.error("Error save:", error);
            alert(`Gagal menyimpan data: ${error.message}`);
        }
    }

    // Panggil Endpoint: /Type/Delete?id={id}
    async function deleteData(id) {
        if (confirm('Apakah Anda yakin ingin menghapus data tipe ini?')) {
            try {
                await webCall(`/Type/Delete?id=${id}`, 'DELETE');
                alert('Data berhasil dihapus.');

                const currentRows = tbody.querySelectorAll('tr').length;
                if (currentRows === 1 && state.page > 1) {
                    state.page--;
                }

                loadData();
            } catch (error) {
                alert(`Gagal menghapus data: ${error.message}`);
            }
        }
    }

    return {
        init: init,
        search: search,
        changePage: changePage,
        showAddModal: showAddModal,
        showEditModal: showEditModal,
        hideModal: hideModal,
        save: save,
        deleteData: deleteData
    };
})();

document.addEventListener("DOMContentLoaded", function () {
    typeController.init();
});