document.addEventListener("DOMContentLoaded", function () {

    // ========================
    // Validation Functions
    // ========================
    function validateFullName(name) {
        // At least two words, letters only
        const regex = /^[A-Za-z]+(?: [A-Za-z]+)+$/;
        return regex.test(name.trim());
    }
    function validateEmail(email) {
        // Only allow Gmail addresses
        const regex = /^[a-zA-Z0-9._%+-]+@gmail.com$/;
        return regex.test(email.trim());
    }

    function showError(inputElement, message) {
        const span = inputElement.parentElement.querySelector(".text-danger");
        if (span) span.textContent = message;
    }

    function clearError(inputElement) {
        const span = inputElement.parentElement.querySelector(".text-danger");
        if (span) span.textContent = "";
    }

    // ========================
    // CREATE MODAL
    // ========================
    const createModal = document.getElementById("adminModal");
    const createForm = document.getElementById("adminForm");
    const openBtn = document.querySelector(".open-modal");
    const createCloseBtn = createModal.querySelector(".close-btn");
    const createCancelBtn = createModal.querySelector(".btn-cancel");
    const createFullName = document.getElementById("Input_FullName");
    const createEmail = document.getElementById("Input_Email");
    const createRole = document.getElementById("Input_Role");

    openBtn?.addEventListener("click", () => {
        createModal.classList.add("show");
        createForm.action = "?handler=CreateAdmin";
        createFullName.value = "";
        createEmail.value = "";
        createRole.value = "Staff Admin";
    });

    const closeCreateModal = () => {
        createModal.classList.remove("show");
        createForm.reset();
        document.querySelectorAll("#adminForm .text-danger").forEach(span => span.textContent = "");
    };
    createCloseBtn.addEventListener("click", closeCreateModal);
    createCancelBtn.addEventListener("click", closeCreateModal);
    window.addEventListener("click", e => { if (e.target === createModal) closeCreateModal(); });

    // Validate on Create submit
    createForm.addEventListener("submit", function (e) {
        let valid = true;

        clearError(createFullName);
        clearError(createEmail);

        if (!validateFullName(createFullName.value)) {
            showError(createFullName, "Full Name must be at least two words, letters only.");
            valid = false;
        }

        if (!validateEmail(createEmail.value)) {
            showError(createEmail, "Please enter a valid email address.");
            valid = false;
        }

        if (!valid) e.preventDefault();
    });

    // ========================
    // EDIT MODAL
    // ========================
    const editModal = document.getElementById("editAdminModal");
    const editForm = document.getElementById("editAdminForm");
    const editCloseBtn = editModal.querySelector(".close-btn");
    const editCancelBtn = editModal.querySelector(".btn-cancel");
    const editFullNameInput = document.getElementById("EditInput_FullName");
    const editEmailInput = document.getElementById("EditInput_Email");

    const editButtons = document.querySelectorAll(".icon-btn.edit");
    editButtons.forEach(btn => {
        btn.addEventListener("click", () => {
            editModal.classList.add("show");
            editForm.action = "?handler=Edit";

            // Populate form fields
            document.getElementById("EditInput_Id").value = btn.dataset.id;
            editFullNameInput.value = btn.dataset.name;
            editEmailInput.value = btn.dataset.email;
            document.getElementById("EditInput_Role").value = btn.dataset.role;
        });
    });

    const closeEditModal = () => {
        editModal.classList.remove("show");
        editForm.reset();
        document.querySelectorAll("#editAdminForm .text-danger").forEach(span => span.textContent = "");
    };
    editCloseBtn.addEventListener("click", closeEditModal);
    editCancelBtn.addEventListener("click", closeEditModal);
    window.addEventListener("click", e => { if (e.target === editModal) closeEditModal(); });

    // Validate on Edit submit
    editForm.addEventListener("submit", function (e) {
        let valid = true;

        clearError(editFullNameInput);
        clearError(editEmailInput);

        if (!validateFullName(editFullNameInput.value)) {
            showError(editFullNameInput, "Full Name must be at least two words, letters only.");
            valid = false;
        }

        if (!validateEmail(editEmailInput.value)) {
            showError(editEmailInput, "Please enter a valid email address.");
            valid = false;
        }

        if (!valid) e.preventDefault();
    });

    // ========================
    // DELETE MODAL
    // ========================
    const deleteModal = document.getElementById("deleteModal");
    const deleteButtons = document.querySelectorAll(".icon-btn.delete");
    const cancelDeleteBtn = deleteModal.querySelector(".btn-cancel-delete");
    const confirmDeleteBtn = deleteModal.querySelector(".btn-delete-confirm");
    const deleteForm = document.getElementById("deleteForm");
    const deleteUserIdInput = document.getElementById("deleteUserId");
    let selectedUserId = null;

    deleteButtons.forEach(btn => {
        btn.addEventListener("click", () => {
            selectedUserId = btn.dataset.id;
            deleteModal.classList.add("show");
        });
    });

    cancelDeleteBtn.addEventListener("click", () => {
        deleteModal.classList.remove("show");
        selectedUserId = null;
    });

    confirmDeleteBtn.addEventListener("click", () => {
        if (!selectedUserId) return;
        deleteUserIdInput.value = selectedUserId;
        deleteForm.action = "?handler=Delete";
        deleteForm.submit();
    });

    window.addEventListener("click", e => {
        if (e.target === deleteModal) {
            deleteModal.classList.remove("show");
            selectedUserId = null;
        }
    });

    // ========================
    // ROLE FILTER
    // ========================
    const searchInput = document.getElementById("searchInput");
    const roleFilter = document.getElementById("roleFilter");
    const tableRows = document.querySelectorAll(".user-table tbody tr");

    function filterTable() {
        const searchTerm = searchInput.value.toLowerCase();
        const selectedRole = roleFilter.value;

        tableRows.forEach(row => {
            const name = row.querySelector(".user-name").textContent.toLowerCase();
            const roleElement = row.querySelector(".role-badge");
            const role = roleElement ? roleElement.textContent.trim() : "";

            row.style.display = (name.includes(searchTerm) && (selectedRole === "" || role === selectedRole)) ? "" : "none";
        });
    }

    searchInput.addEventListener("input", filterTable);
    roleFilter.addEventListener("change", filterTable);

    // ========================
    // LIVE FULL NAME INPUT RESTRICTION
    // ========================
    if (createFullName) {
        createFullName.addEventListener("input", function () {
            this.value = this.value.replace(/[^a-zA-Z\s]/g, '');
        });
    }
    if (editFullNameInput) {
        editFullNameInput.addEventListener("input", function () {
            this.value = this.value.replace(/[^a-zA-Z\s]/g, '');
        });
    }

});