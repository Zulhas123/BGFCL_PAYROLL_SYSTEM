$(document).ready(function () {
    loadInitialData();
});

function loadInitialData() {
    // Load Employee Types
    $.ajax({
        url: '/api/Employees/GetEmployeeTypes',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let employeeTypes = responseObject.data;
            $('#EmployeeTypeId').empty();
            $('#EmployeeTypeId').append('<option value="0">Select One</option>');
            $.each(employeeTypes, function (key, item) {
                $('#EmployeeTypeId').append(`<option value="${item.id}">${item.employeeTypeName}</option>`);
            });
        },
        error: function (xhr, status, error) {
            console.error("Error fetching employee types:", error);
        }
    });
}

function loadSelectedFields() {
    const selected = Array.from(document.querySelectorAll('.select-field-checkbox:checked'))
        .map(chk => chk.value);

    const container = document.getElementById('fieldConfigContainer');
    const form = document.getElementById('fieldConfigForm');
    form.innerHTML = "";

    if (selected.length === 0) {
        form.innerHTML = '<p class="text-muted">No fields selected.</p>';
        return;
    }

    let rowDiv = document.createElement('div');
    rowDiv.className = 'row g-4';

    selected.forEach((field) => {
        const colDiv = document.createElement('div');
        colDiv.className = 'col-md-6';

        let isEmployeeType = field === "EmployeeTypeId";
        let dropdownHtml = `
            <select class="form-select" id="ddl_${field}" onchange="${isEmployeeType ? 'applyEmployeeTypeToAll(this.value)' : ''}">
                <option value="">Select Option</option>
            </select>`;

        let configHtml = `
            <div class="p-3 border rounded shadow-sm">
                <div class="fw-bold mb-2">${field}</div>
                <div class="form-check form-check-inline">
                    <input class="form-check-input" type="radio" name="option_${field}" id="optText_${field}" data-type="text"
                        onchange="toggleInput('${field}', 'text')" ${isEmployeeType ? 'disabled' : ''}>
                    <label class="form-check-label" for="optText_${field}">Textbox</label>
                </div>
                <div class="form-check form-check-inline">
                    <input class="form-check-input" type="radio" name="option_${field}" id="optDrop_${field}" data-type="dropdown"
                        onchange="toggleInput('${field}', 'dropdown')" ${isEmployeeType ? 'checked' : ''}>
                    <label class="form-check-label" for="optDrop_${field}">Dropdown</label>
                </div>
                <div class="mt-2 input-wrapper ${isEmployeeType ? '' : 'd-none'}" id="inputDropWrapper_${field}">
                    ${dropdownHtml}
                </div>
                <div class="mt-2 input-wrapper ${isEmployeeType ? 'd-none' : ''}" id="inputTextWrapper_${field}">
                    <input type="text" class="form-control" id="txt_${field}" placeholder="Enter value" />
                </div>
            </div>`;

        colDiv.innerHTML = configHtml;
        rowDiv.appendChild(colDiv);

        // ✅ Populate EmployeeTypeId dropdown dynamically from API
        if (isEmployeeType) {
            $.ajax({
                url: '/api/Employees/GetEmployeeTypes',
                type: 'GET',
                dataType: 'json',
                success: function (responseObject) {
                    let employeeTypes = responseObject.data;
                    const ddl = document.getElementById(`ddl_${field}`);
                    employeeTypes.forEach(item => {
                        ddl.innerHTML += `<option value="${item.id}">${item.employeeTypeName}</option>`;
                    });
                },
                error: function (xhr, status, error) {
                    console.error("Error fetching employee types for dropdown:", error);
                }
            });
        }
    });

    form.appendChild(rowDiv);

    const submitBtn = document.createElement('button');
    submitBtn.className = "btn btn-success mt-4";
    submitBtn.type = "button";
    submitBtn.textContent = "Save Configuration";
    submitBtn.onclick = submitFinalConfig;
    form.appendChild(submitBtn);
}

function toggleInput(field, type) {
    const textWrapper = document.getElementById(`inputTextWrapper_${field}`);
    const dropWrapper = document.getElementById(`inputDropWrapper_${field}`);

    if (textWrapper && dropWrapper) {
        textWrapper.classList.toggle('d-none', type !== 'text');
        dropWrapper.classList.toggle('d-none', type !== 'dropdown');
    }
}

function applyEmployeeTypeToAll(value) {
    document.querySelectorAll('select[id^="ddl_"]').forEach(select => {
        const field = select.id.replace("ddl_", "");
        if (field === "EmployeeTypeId") return;
        const inputTypeRadio = document.querySelector(`#optDrop_${field}`);
        if (inputTypeRadio?.checked) {
            select.value = value;
        }
    });
}

function submitFinalConfig() {
    const results = [];
    let employeeTypeIdValue = null;

    document.querySelectorAll('#fieldConfigForm .col-md-6').forEach(wrapper => {
        const field = wrapper.querySelector('.fw-bold')?.textContent.trim();
        const isText = wrapper.querySelector(`#optText_${field}`)?.checked;
        const isDrop = wrapper.querySelector(`#optDrop_${field}`)?.checked;

        let inputType = null;
        let value = null;

        if (isText) {
            inputType = "textbox";
            value = wrapper.querySelector(`#txt_${field}`)?.value;
        } else if (isDrop) {
            inputType = "dropdown";
            value = wrapper.querySelector(`#ddl_${field}`)?.value;
        }

        if (inputType && value !== null) {
            const item = {
                field,
                inputType,
                value,
                employeeTypeId: 0 // Will be updated
            };

            if (field === "EmployeeTypeId") {
                employeeTypeIdValue = parseInt(value);
            }

            results.push(item);
        }
    });

    // Apply the employeeTypeId to ALL items
    if (employeeTypeIdValue !== null) {
        results.forEach(item => {
            item.employeeTypeId = employeeTypeIdValue;
        });
    }

    console.log("Final payload:", results);

    $.ajax({
        url: '/api/SalarySettings/SalarySettinsConfig',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(results),
        success: function (res) {
            if (res && res.statusCode === 200) {
                alert("Saved Successfully!");
            } else {
                alert("Failed to save: " + (res.responseMessage || "Unknown error."));
            }
        },
        error: function (xhr, status, error) {
            console.error("Error:", status, error);
            alert("An error occurred while saving the data.");
        }
    });
}
