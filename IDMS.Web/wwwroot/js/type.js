let state = {
    page: 1,
    search: '',
    limit: 10 // Coba ubah ke 2 untuk ngetest pagination berjalan
};

// let nextId = 4;

let searchTimer;
let currentDeleteId = null;

document.addEventListener('DOMContentLoaded', () => {
    loadBrandDropdown();
    loadData();
});

function renderPagination(pagination) {
    const container = document.getElementById('pagination');

    // Sembunyikan pagination jika data kosong
    if (pagination.totalItems === 0 || pagination.totalItems <= state.limit) {
        container.classList.add('hidden');
        return;
    }

    container.classList.remove('hidden');

    // Kalkulasi item yang sedang ditampilkan (misal: "Showing 1 to 10 of 25")
    const startItem = ((pagination.currentPage - 1) * pagination.limit) + 1;
    const endItem = Math.min(pagination.currentPage * pagination.limit, pagination.totalItems);

    let html = `
        <div class="text-sm text-gray-500">
            Showing <span class="font-medium text-gray-900">${startItem}</span> to <span class="font-medium text-gray-900">${endItem}</span> of <span class="font-medium text-gray-900">${pagination.totalItems}</span> results
        </div>
        <div class="flex items-center gap-2">
    `;

    // Render Tombol "Previous"
    if (pagination.hasPreviousPage) {
        html += `<button onclick="changePage(${pagination.currentPage - 1})" class="px-3 py-1.5 text-sm font-medium text-gray-700 bg-white border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors">Previous</button>`;
    } else {
        html += `<button disabled class="px-3 py-1.5 text-sm font-medium text-gray-400 bg-gray-50 border border-gray-200 rounded-lg cursor-not-allowed">Previous</button>`;
    }

    // Render Info Halaman (Page X of Y)
    html += `<span class="text-sm font-medium text-gray-700 px-2">Page ${pagination.currentPage} of ${pagination.totalPages}</span>`;

    // Render Tombol "Next"
    if (pagination.hasNextPage) {
        html += `<button onclick="changePage(${pagination.currentPage + 1})" class="px-3 py-1.5 text-sm font-medium text-gray-700 bg-white border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors">Next</button>`;
    } else {
        html += `<button disabled class="px-3 py-1.5 text-sm font-medium text-gray-400 bg-gray-50 border border-gray-200 rounded-lg cursor-not-allowed">Next</button>`;
    }

    html += `</div>`;
    container.innerHTML = html;
}

// ================= GANTI HALAMAN =================
function changePage(newPage) {
    state.page = newPage;
    loadData(); // Hit API lagi berdasarkan page yang baru
}

// ================= GET BRANDS UNTUK DROPDOWN =================
async function loadBrandDropdown() {
    try {
        // MVC BrandController akan merespons dengan JSON array Brand
        // Header Bearer & X-Api-Key di-handle otomatis oleh ApiClient.cs saat BrandService memanggil BE API
        const response = await fetch('/Brand/List?limit=1000');
        const result = await response.json();

        const select = document.getElementById('brandId');
        if (result.status.toLowerCase() === "success") {
            // Mapping opsi menjadi "Code | Name" sesuai request kamu
            result.data.forEach(b => {
                select.innerHTML += `<option value="${b.id}">${b.code} | ${b.name}</option>`;
            });
        }
    } catch (error) {
        console.error("Error loading brand dropdown:", error);
    }
}

// ================= 1. FUNGSI SAAT USER MENGETIK (DEBOUNCE) =================
function onSearchInput() {
    clearTimeout(searchTimer);
    const input = document.getElementById('searchInput');
    document.getElementById('clearBtn').classList.toggle('hidden', input.value.length === 0);

    // Tunggu 500ms setelah user selesai mengetik
    searchTimer = setTimeout(() => {
        state.search = input.value.trim(); // Simpan keyword ke state
        state.page = 1; // Wajib reset ke page 1 tiap kali pencarian baru
        loadData();
    }, 500);
}

function clearSearch() {
    const input = document.getElementById('searchInput');
    input.value = '';
    document.getElementById('clearBtn').classList.add('hidden');

    state.search = '';
    state.page = 1; // Reset ke page 1
    loadData();
    input.focus();
}

// ================= 3. FUNGSI LOAD DATA (TERMASUK SEARCH) =================
// Tambahkan parameter page dan limit agar fleksibel
async function loadData() {
    const tbody = document.getElementById('typeTableBody');
    tbody.innerHTML = `<tr><td colspan="6" class="py-16 text-center text-gray-400 text-sm">Loading data...</td></tr>`;

    try {
        // Build URL menggunakan state
        let url = `/Type/List?page=${state.page}&limit=${state.limit}`;
        if (state.search !== '') {
            url += `&keyword=${encodeURIComponent(state.search)}`;
        }

        const response = await fetch(url);
        const result = await response.json();

        if (result.status.toLowerCase() === "success") {
            if (!result.data || result.data.length === 0) {
                tbody.innerHTML = `<tr><td colspan="6" class="py-16 text-center text-gray-500 text-sm">No types found.</td></tr>`;
                document.getElementById('pagination').classList.add('hidden');
                return;
            }

            // Render Data ke Table
            tbody.innerHTML = result.data.map(item => `
                <tr class="hover:bg-gray-50/50 transition-colors">
                    <td class="py-3 px-4 text-sm text-gray-900 font-semibold">${item.brandName}</td>
                    <td class="py-3 px-4 text-sm text-gray-900">${item.code}</td>
                    <td class="py-3 px-4 text-sm text-gray-500">${item.name}</td>
                    <td class="py-3 px-4 text-sm text-gray-500">${item.year}</td>
                    <td class="py-3 px-4">
                        <span class="inline-flex items-center px-2 py-1 rounded-md text-xs font-medium ${item.isActive ? 'bg-green-50 text-green-700 ring-green-600/20' : 'bg-red-50 text-red-700 ring-red-600/20'} ring-1 ring-inset">
                            ${item.isActive ? 'Active' : 'Inactive'}
                        </span>
                    </td>
                    <td class="py-3 px-4 text-sm text-gray-500">
                        <div class="flex items-center gap-3">
                            <button onclick="openEditModal(${item.id})" class="text-indigo-600 hover:text-indigo-900 font-medium transition-colors">Edit</button>
                            <button onclick="openDeleteModal(${item.id}, '${item.code} - ${item.name}')" class="text-red-600 hover:text-red-900 font-medium transition-colors">Delete</button>
                        </div>
                    </td>
                </tr>
            `).join('');

            // Panggil fungsi render pagination
            if (result.pagination) {
                renderPagination(result.pagination);
            }

        } else {
            tbody.innerHTML = `<tr><td colspan="6" class="py-16 text-center text-red-500 text-sm">${result.message || 'Failed to load data.'}</td></tr>`;
            document.getElementById('pagination').classList.add('hidden');
        }
    } catch (error) {
        console.error("API Error:", error);
        tbody.innerHTML = `<tr><td colspan="6" class="py-16 text-center text-red-500 text-sm">Connection error to server.</td></tr>`;
        document.getElementById('pagination').classList.add('hidden');
    }
}
// ================= MODAL CONTROLS =================
function openCreateModal() {
    document.getElementById('modalTitle').textContent = 'Create Type';
    document.getElementById('typeId').value = '';
    document.getElementById('brandId').value = '';
    document.getElementById('typeCode').value = '';
    document.getElementById('typeName').value = '';
    document.getElementById('typeYear').value = '';
    document.getElementById('modalError').classList.add('hidden');

    document.getElementById('modalOverlay').classList.replace('hidden', 'flex');
}

async function openEditModal(id) {
    try {
        // Melakukan Fetch GET ke MVC Controller -> Diteruskan ke /api/type/{id}
        const response = await fetch(`/Type/Detail?id=${id}`);
        const result = await response.json();

        // Cek status response case-insensitive
        if (result.status.toLowerCase() === "success") {
            document.getElementById('modalTitle').textContent = 'Edit Type';

            // Auto-fill dialog form dengan data dari database
            document.getElementById('typeId').value = result.data.id;
            document.getElementById('brandId').value = result.data.brandId;
            document.getElementById('typeCode').value = result.data.code;
            document.getElementById('typeName').value = result.data.name;
            document.getElementById('typeYear').value = result.data.year;

            // Sembunyikan error dan tampilkan modal
            document.getElementById('modalError').classList.add('hidden');
            document.getElementById('modalOverlay').classList.replace('hidden', 'flex');
        } else {
            alert(result.message || "Data not found.");
        }
    } catch (error) {
        console.error("Error fetching detail:", error);
        alert("Failed to load type detail from server.");
    }
}

function closeModal() {
    document.getElementById('modalOverlay').classList.replace('flex', 'hidden');
}

// ================= POST / PUT (SAVE) =================
async function submitForm() {
    const id = document.getElementById('typeId').value;
    const brandId = document.getElementById('brandId').value;
    const code = document.getElementById('typeCode').value.trim();
    const name = document.getElementById('typeName').value.trim();
    const year = document.getElementById('typeYear').value;

    const errorBox = document.getElementById('modalError');
    const errorText = document.getElementById('modalErrorText');
    const saveBtn = document.getElementById('saveBtn');

    // Validasi kosong
    if (!brandId || !code || !name || !year) {
        errorText.textContent = "All fields are required.";
        errorBox.classList.remove('hidden');
        return;
    }


    if (code.length > 3){
        errorText.textContent = "Code cannot more than 3 Char";
        errorBox.classList.remove('hidden');
        return;
    }

    // Validasi Tahun
    if (parseInt(year) < 1900) {
        errorText.textContent = "Year must be 1900 or greater.";
        errorBox.classList.remove('hidden');
        return;
    }

    // Siapkan JSON Payload sesuai Request BE
    const payload = {
        brandId: parseInt(brandId),
        code: code.toUpperCase(),
        name: name,
        year: parseInt(year),
        isActive: true // Selalu true sesuai requirement
    };

    saveBtn.disabled = true;
    saveBtn.textContent = "Saving...";

    try {
        // Tentukan Method dan URL
        const url = id ? `/Type/Update?id=${id}` : '/Type/Create';
        const method = id ? 'PUT' : 'POST';

        // Lakukan Fetch Request
        const response = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        const result = await response.json();

        // Sukses Simpan
        if (result.status.toLowerCase() === "success") {
            closeModal();
            loadData(); // Refresh Data Tabel Types
        } else {
            // Tampilkan error validasi dari BE
            errorText.textContent = result.message || "Failed to save data.";
            errorBox.classList.remove('hidden');
        }
    } catch (error) {
        console.error("Save Error:", error);
        errorText.textContent = "Failed to communicate with server.";
        errorBox.classList.remove('hidden');
    } finally {
        // Kembalikan kondisi tombol
        saveBtn.disabled = false;
        saveBtn.textContent = "Save";
    }
}

// ================= DELETE =================
// ================= DELETE DATA =================
// 1. Fungsi ini dipanggil dari tombol "Delete" di dalam tabel untuk memunculkan konfirmasi
function openDeleteModal(id, info) {
    currentDeleteId = id; // Simpan ID yang mau dihapus di memori
    document.getElementById('deleteTypeInfo').textContent = info; // Tampilkan nama item di modal
    document.getElementById('deleteError').classList.add('hidden');

    // Tampilkan Modal Konfirmasi
    const modal = document.getElementById('deleteModalOverlay');
    modal.classList.remove('hidden');
    modal.classList.add('flex');
}

// 2. Fungsi untuk menutup modal konfirmasi
function closeDeleteModal() {
    currentDeleteId = null;
    const modal = document.getElementById('deleteModalOverlay');
    modal.classList.add('hidden');
    modal.classList.remove('flex');
}

// 3. Fungsi ini dipanggil saat tombol merah "Delete" di dalam modal diklik
async function confirmDelete() {
    if (!currentDeleteId) return;

    const btnConfirm = document.getElementById('confirmDeleteBtn');

    // Set tombol ke state loading agar user tidak bisa double click
    btnConfirm.disabled = true;
    btnConfirm.textContent = "Deleting...";

    try {
        // Melakukan Fetch DELETE ke MVC Controller
        const response = await fetch(`/Type/Delete?id=${currentDeleteId}`, {
            method: 'DELETE',
            headers: { 'Content-Type': 'application/json' }
        });

        const result = await response.json();

        // Cek jika response status dari controller adalah Success
        if (result.status.toLowerCase() === "success") {
            closeDeleteModal(); // Tutup modal
            loadData(); // Panggil ulang data tabel (refresh UI)
        } else {
            // Tampilkan pesan error dari Backend ke dalam modal
            document.getElementById('deleteErrorText').textContent = result.message || "Failed to delete data.";
            document.getElementById('deleteError').classList.remove('hidden');
        }
    } catch (error) {
        console.error("Delete Error:", error);
        document.getElementById('deleteErrorText').textContent = "Connection error to server.";
        document.getElementById('deleteError').classList.remove('hidden');
    } finally {
        // Kembalikan state tombol
        btnConfirm.disabled = false;
        btnConfirm.textContent = "Delete";
    }
}