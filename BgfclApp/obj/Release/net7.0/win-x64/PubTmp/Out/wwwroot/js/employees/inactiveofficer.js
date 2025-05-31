let activeStatus = [];
let _dataTable = undefined;

$(document).ready(function () {
    loadInitialData();

    let pageLength = parseInt($('#page_length_select').val());
    let selectedEmployee = $('#filter_select').val(); 

    // Load employee list based on initial settings
    getEmployeeList(pageLength, selectedEmployee);

    // Fetch inactive employees for the filter select dropdown
    $.ajax({
        url: '/api/Employees/GetInactiveEmployees?employeeTypeId=1',
        method: 'GET',
        success: function (response) {
            var employees = response.data;
            employees.forEach(function (employee) {
                $('#filter_select').append(
                    $('<option>', {
                        value: employee.id,
                        text: employee.employeeName
                    })
                );
            });
        },
        error: function (xhr, status, error) {
            console.error('Failed to fetch employees:', error);
        }
    });

    // On filter select change, reload the employee list with the selected employee filter
    $('#filter_select').on('change', function () {
        selectedEmployee = $(this).val(); // Get selected employee ID
        getEmployeeList(pageLength, selectedEmployee);  // Pass the selected employee and page length
    });

    // On page length change, reload the employee list while maintaining the filter
    $('#page_length_select').on('change', function () {
        pageLength = parseInt($(this).val());
        getEmployeeList(pageLength, selectedEmployee);  // Pass the updated page length and current employee filter
    });

    // Filter functionality for a search input
    $('#filter_input').on('keyup', function () {
        _dataTable.search(this.value).draw();
    });
});

// Load initial data like active statuses
function loadInitialData() {
    $.ajax({
        url: '/api/Others/GetActiveStatus',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            activeStatus = responseObject.data;
        },
        error: function (responseObject) {
            console.error('Error loading active status', responseObject);
        }
    });
}

// Function to get the employee list with pagination and filtering by officer
function getEmployeeList(pageLength, selectedEmployee) {
    if (_dataTable != undefined) {
        _dataTable.destroy();
    }

    // Initialize DataTable with the provided page length and employee filter
    _dataTable = $('#employee_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url: '/api/Employees/GetInactiveEmployees?employeeTypeId=1',
            data: function (d) {
                d.employeeId = selectedEmployee; // Send selected employee ID as a filter to the backend
            },
            dataSrc: 'data'
        },
        columns: [
            {
                data: '',
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: 'jobCode' },
            { data: 'employeeName' },
            { data: 'designationName' },
            { data: 'departmentName' },
            {
                data: 'id',
                render: (data, type, row) => {
                    return `<button type="button" class="btn btn-primary btn-sm" onclick="onEditClicked(${data})">Edit</button>`;
                }
            }
        ],
        dom: '<"d-flex justify-content-between align-items-center"Bf>tip',
        buttons: [
            'copy', 'excel', 'pdf', 'print'
        ]
    });
}


function onEditClicked(employeeId) {
    $('#EmployeeId').val(employeeId);

    $.ajax({
        url: `/api/Employees/GetEmployeeById?id=${employeeId}`,
        type: 'GET',
        success: function (responseObject) {
            if (responseObject.statusCode === 200) {
                const employee = responseObject.data;
                $('#JobCode').val(employee.jobCode);
                $('#EmployeeName').val(employee.employeeName);
                $('#ActiveStatus').empty();
                $.each(activeStatus, function (key, item) {
                    if (item.id == employee.activeStatus) {
                        $('#ActiveStatus').append(`<option value="${item.id}" selected>${item.activeStatusName}</option>`);
                    } else {
                        $('#ActiveStatus').append(`<option value="${item.id}">${item.activeStatusName}</option>`);
                    }
                });
                $('#edit_modal').modal('show');
            } else {
                showToast('Error', 'Employee details not found', 'error');
            }
        },
        error: function () {
            showToast('Error', 'Unable to fetch employee details', 'error');
        }
    });
}

function onEditConfirmed() {
    let employeeId = $('#EmployeeId').val();
    let jobCode = $('#JobCode').val();
    let employeeName = $('#EmployeeName').val();
    let activeStatus = $('#ActiveStatus').val();

    if (employeeId === '') {
        employeeId = 0;
    }

    let dataObj = {
        id: employeeId,
        jobCode: jobCode,
        employeeName: employeeName,
        activeStatus: activeStatus
    };

    $.ajax({
        url: '/api/Employees/UpdateInactiveEmployee',
        type: 'PUT',
        async: false,
        data: dataObj,
        success: function (responseObject) {
            $('.error-item').empty();
            if (responseObject.statusCode === 201) {
                showToast('Success', responseObject.responseMessage, 'success');
                $('#edit_modal').modal('hide');
                getEmployeeList();
                
            } else if (responseObject.statusCode === 400) {
                for (let error in responseObject.errors) {
                    $(`#${error}`).empty();
                    $(`#${error}`).append(responseObject.errors[error]);
                }
                showToast('Error', responseObject.responseMessage, 'error');
            } else if (responseObject.statusCode === 500 || responseObject.statusCode === 404) {
                showToast('Error', responseObject.responseMessage, 'error');
            }
        },
        error: function (responseObject) {
            console.error('Error response:', responseObject);
            let errorMessage = responseObject.responseJSON?.responseMessage || 'Failed to update employee data';
            showToast('Error', errorMessage, 'error');
        }
    });
}

