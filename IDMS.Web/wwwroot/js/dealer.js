/**
 * Javascript untuk Master Dealer (Vanilla JS & Tailwind)
 * Menggunakan Global Functions
 */

// --- Variabel Global ---
let keyword = '';
let page = 1;
let limit = 10;

let tbody;
let form;
let modalEl;
let searchInput;
let paginationContainer;

// --- Inisialisasi Saat Halaman Dimuat ---
document.addEventListener("DOMContentLoaded", function () {
    tbody = document.getElementById('dealerTableBody');
    form = document.getElementById('dealerForm');
    modalEl = document.getElementById('dealerModal');
    searchInput = document.getElementById('searchInput');
    paginationContainer = document.getElementById('paginationContainer');

    loadData();

    // Trigger pencarian
    searchInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') search();
    });
});

// --- Wrapper Fetch ---
async function webCall(url, method = 'GET', data = null) {
    const options = {
        method: method,
        headers: { 'Content-Type': 'application/json' }
    };

    if (data) options.body = JSON.stringify(data);

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

// --- Fungsi Utama ---
async function loadData() {
    tbody.innerHTML = '<tr><td colspan="6" class="px-6 py-8 text-center text-sm text-gray-500">Memuat data...</td></tr>';

    const queryUrl = `/Dealer/List?keyword=${encodeURIComponent(keyword)}&page=${page}&limit=${limit}`;

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
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td class="px-6 py-4 whitespace-nowrap">
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800 uppercase">
                    ${item.code}
                </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 font-medium">${item.name}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                <div>${item.city}</div>
                <div class="text-xs text-gray-400">${item.region}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                <div>${item.phone || '-'}</div>
                <div class="text-xs text-gray-400">${item.email || '-'}</div>
            </td>
            <td class="px-6 py-4 text-sm text-gray-500 truncate max-w-xs">${item.address || '-'}</td>
            <td class="px-6 py-4 whitespace-nowrap text-center text-sm font-medium">
                <button onclick="showEditModal(${item.id})" class="text-indigo-600 hover:text-indigo-900 bg-indigo-50 hover:bg-indigo-100 px-3 py-1 rounded-md mr-2 transition-colors">Edit</button>
                <button onclick="deleteData(${item.id})" class="text-red-600 hover:text-red-900 bg-red-50 hover:bg-red-100 px-3 py-1 rounded-md transition-colors">Hapus</button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

function renderPagination(pagination) {
    const totalPages = pagination?.totalPages || pagination?.TotalPages || 0;
    const currentPage = pagination?.currentPage || pagination?.CurrentPage || page;
    const totalItems = pagination?.totalItems || pagination?.TotalItems || 0;

    if (totalPages <= 1) {
        paginationContainer.innerHTML = '';
        paginationContainer.classList.add('hidden');
        return;
    }

    paginationContainer.classList.remove('hidden');

    const prevDisabled = currentPage === 1 ? 'disabled class="opacity-50 cursor-not-allowed"' : `onclick="changePage(${currentPage - 1})" class="hover:bg-gray-50 cursor-pointer"`;
    const nextDisabled = currentPage === totalPages ? 'disabled class="opacity-50 cursor-not-allowed"' : `onclick="changePage(${currentPage + 1})" class="hover:bg-gray-50 cursor-pointer"`;

    let pageButtons = '';
    for (let i = 1; i <= totalPages; i++) {
        if (i === currentPage) {
            pageButtons += `<button aria-current="page" class="relative z-10 inline-flex items-center bg-indigo-600 px-4 py-2 text-sm font-semibold text-white focus:z-20 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600">${i}</button>`;
        } else {
            pageButtons += `<button onclick="changePage(${i})" class="relative inline-flex items-center px-4 py-2 text-sm font-semibold text-gray-900 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0">${i}</button>`;
        }
    }

    const startItem = ((currentPage - 1) * limit) + 1;
    const endItem = Math.min(currentPage * limit, totalItems);

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

function changePage(newPage) {
    page = newPage;
    loadData();
}

function search() {
    keyword = searchInput.value;
    page = 1;
    loadData();
}

// --- Kontrol Modal ---
function showModal() { modalEl.classList.remove('hidden'); }
function hideModal() { modalEl.classList.add('hidden'); }

function showAddModal() {
    form.reset();
    document.getElementById('dealerId').value = '';
    document.getElementById('dealerModalLabel').innerText = 'Tambah Dealer Baru';
    showModal();
}

async function showEditModal(id) {
    try {
        const response = await webCall(`/Dealer/Detail?id=${id}`);
        const data = response.data;

        document.getElementById('dealerId').value = data.id;
        document.getElementById('dealerCode').value = data.code || '';
        document.getElementById('dealerName').value = data.name || '';
        document.getElementById('dealerCity').value = data.city || '';
        document.getElementById('dealerRegion').value = data.region || '';
        document.getElementById('dealerPhone').value = data.phone || '';
        document.getElementById('dealerEmail').value = data.email || '';
        document.getElementById('dealerAddress').value = data.address || '';

        document.getElementById('dealerModalLabel').innerText = 'Edit Dealer';
        showModal();
    } catch (error) {
        alert(`Gagal mengambil data: ${error.message}`);
    }
}

// --- Fungsi Simpan & Hapus ---
async function save() {
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const id = document.getElementById('dealerId').value;
    const isEdit = id !== '';

    const payload = {
        code: document.getElementById('dealerCode').value.toUpperCase(),
        name: document.getElementById('dealerName').value,
        city: document.getElementById('dealerCity').value,
        region: document.getElementById('dealerRegion').value,
        phone: document.getElementById('dealerPhone').value,
        email: document.getElementById('dealerEmail').value,
        address: document.getElementById('dealerAddress').value
    };

    const method = isEdit ? 'PUT' : 'POST';
    const url = isEdit ? `/Dealer/Update?id=${id}` : `/Dealer/Create`;

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

async function deleteData(id) {
    if (confirm('Apakah Anda yakin ingin menghapus data dealer ini?')) {
        try {
            await webCall(`/Dealer/Delete?id=${id}`, 'DELETE');
            alert('Data berhasil dihapus.');

            const currentRows = tbody.querySelectorAll('tr').length;
            if (currentRows === 1 && page > 1) {
                page--;
            }

            loadData();
        } catch (error) {
            alert(`Gagal menghapus data: ${error.message}`);
        }
    }
}