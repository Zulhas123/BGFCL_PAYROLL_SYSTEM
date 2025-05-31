let _dataTable = undefined;
$(document).ready(function () {
    loadInitialData();

    $('#Salary_process_button').on('click', function () {
        console.log("Salary Process Button Clicked!");
        $('#process_modal').modal('show');
        $('#ProcessId').val('');
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#ProcessModalLabel').html('Salary Process');
    });

    //$('#process_button').on('click', function () {


    //    let _monthId = getMonthId(); // Get selected month
    //    let _employeeTypeId = $('#EmployeeTypeId').val(); // Get employee type

    //    $.ajax({
    //        url: '/api/SalarySettings/GetSalaryProcessMaster',
    //        type: 'POST',
    //        contentType: 'application/json',
    //        data: JSON.stringify({ SalaryProcessingID: processId }),
    //        dataType: 'json',
    //        success: function (responseObject) {
    //            alert(responseObject.responseMessage);
    //        },
    //        error: function (xhr) {
    //            console.error('Error:', xhr);
    //            alert('Failed to update salary process Master data.');
    //        }
    //    });


    //    $.ajax({
    //        url: `/api/SalarySettings/GetSalaryProcess`,
    //        type: 'GET',
    //        dataType: 'json',
    //        success: function (response) {
    //            console.log("API Response:", response);

    //            if (!response.data || !Array.isArray(response.data)) {
    //                console.error("Invalid API response format:", response);
    //                return;
    //            }

    //            let _monthIdStr = String(_monthId); // Ensure it's a string
    //            let year = _monthIdStr.substring(0, 4); // Extract year
    //            let monthName = getMonthName(_monthIdStr.substring(4, 6)); // Convert to month name

    //            let existingProcess = response.data.find(item =>
    //                item.year == year &&
    //                item.month == monthName &&
    //                item.employeeType == $('#EmployeeTypeId option:selected').text() // Match Employee Type Name
    //            );

    //            console.log("Existing Process Found:", existingProcess);

    //            if (existingProcess) {
    //                // Show message before asking to re-process
    //                showToast("Warning", `Salary process for ${monthName} ${year} (${existingProcess.employeeType}) already exists!`, "warning");

    //                // Confirm re-processing action
    //                showConfirmationDialog(
    //                    "Already processed for this month and year. Do you want to re-process?",
    //                    function () {
    //                        callSalaryProcessApi(_monthId, _employeeTypeId); // Call API to re-process
    //                        $('#process_modal').modal('hide'); // Hide modal after re-processing
    //                    },
    //                    function () {
    //                        $('#process_modal').modal('hide'); // Hide the modal if user cancels
    //                    }
    //                );
    //            } else {
    //                // No existing process, proceed to salary process
    //                callSalaryProcessApi(_monthId, _employeeTypeId);
    //            }
    //        },
    //        error: function () {
    //            console.error("API call failed!");
    //            showToast("Error", "Failed to fetch salary process data.", "error");
    //        }
    //    });
    //});

    $('#process_button').on('click', function () {

        let _monthId = getMonthId(); // Get selected month
        let _employeeTypeId = $('#EmployeeTypeId').val(); // Get employee type

        // 1st API: Check if Salary Processing is stopped
        $.ajax({
            url: '/api/SalarySettings/GetSalaryProcessMaster',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                if (!response.data || !Array.isArray(response.data)) {
                    console.error("Invalid SalaryProcessMaster response format:", response);
                    return;
                }
                console.log("processMasterresponse", response)
                let processMaster = response.data.find(item =>
                    item.monthID == _monthId &&
                    item.employeeTypeID == _employeeTypeId
                );
                console.log("processMaster", processMaster)
                if (processMaster && processMaster.status === 0) {
                    alert(`Salary Process Finally Submitted for this month.`);
                    return; // Stop further processing
                    $('#formId')[0].reset();
                    $('#process_modal').modal('hide');
                }
                console.log("response", response)
                // Proceed to the 2nd API if not stopped
                fetchAndProcessSalary(_monthId, _employeeTypeId);
            },
            error: function () {
                alert('Failed to check salary process master.');
            }
        });
    });

    function fetchAndProcessSalary(_monthId, _employeeTypeId) {
        $.ajax({
            url: `/api/SalarySettings/GetSalaryProcess`,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                console.log("API Response:", response);

                if (!response.data || !Array.isArray(response.data)) {
                    console.error("Invalid API response format:", response);
                    return;
                }

                let _monthIdStr = String(_monthId);
                let year = _monthIdStr.substring(0, 4);
                let monthName = getMonthName(_monthIdStr.substring(4, 6));

                let existingProcess = response.data.find(item =>
                    item.year == year &&
                    item.month == monthName &&
                    item.employeeType == $('#EmployeeTypeId option:selected').text()
                );

                if (existingProcess) {
                    showToast("Warning", `Salary process for ${monthName} ${year} (${existingProcess.employeeType}) already exists!`, "warning");

                    showConfirmationDialog(
                        "Already processed for this month and year. Do you want to re-process?",
                        function () {
                            callSalaryProcessApi(_monthId, _employeeTypeId);
                            $('#process_modal').modal('hide');
                        },
                        function () {
                            $('#process_modal').modal('hide');
                        }
                    );
                } else {
                    callSalaryProcessApi(_monthId, _employeeTypeId);
                }
            },
            error: function () {
                console.error("API call failed!");
                showToast("Error", "Failed to fetch salary process data.", "error");
            }
        });
    }



    $('#cancel_button').on('click', () => {
        $('#formId')[0].reset();
        $('#process_modal').modal('hide');
    });
});

/**
 * Returns the selected MonthID in format YYYYMM (e.g., "202501").
 */
function getMonthId() {
    let year = $('#Year').val();
    let month = $('#Month').val().padStart(2, '0'); // Ensure two digits
    return String(year) + String(month);
}

/**
 * Converts a numeric month (e.g., "01") to a month name (e.g., "January").
 */
function getMonthName(monthNumber) {
    const monthNames = [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    return monthNames[parseInt(monthNumber, 10) - 1]; // Convert "01" -> 0 index
}

/**
 * Calls the Salary Process API to process the salary for the given month and employee type.
 */
function callSalaryProcessApi(monthId, employeeTypeId) {
    console.log(`Calling Salary Process API with MonthID: ${monthId} and EmployeeTypeID: ${employeeTypeId}`);

    // Make the API call to process the salary
    $.ajax({
        url: `/api/SalarySettings/SalaryProcess?monthId=${monthId}&employeeTypeId=${employeeTypeId}`,
        type: 'GET', // Corrected the HTTP method to 'GET'
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode == 201) {
                showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                $('#formId')[0].reset();
                $('#process_modal').modal('hide');
                //getProcessList(pageLength)
                //$('#Salary_process_button').focus(); 
                if (_dataTable) {
                    _dataTable.ajax.reload(null, false); // Reload without resetting pagination
                }
            }
            if (responseObject.statusCode == 500) {
                showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
            }
        },
        error: function (responseObject) {
            showToast(title = 'Error', message = 'An error occurred while processing the request.', toastrType = 'error');
        }
    });
}



/**
 * Displays a confirmation dialog with OK/Cancel buttons.
 */
function showConfirmationDialog(message, onConfirm, onCancel) {
    if (confirm(message)) {
        onConfirm();
    } else {
        onCancel();
    }
}

/**
 * Shows a toast notification.
 */
function showToast(type, message, status) {
    let bgColor = status === "error" ? "red" : status === "warning" ? "orange" : "green";
    let toast = `<div class="toast-message" style="background: ${bgColor}; padding: 10px; border-radius: 5px; color: white;">
                    ${message}
                 </div>`;
    $("body").append(toast);
    setTimeout(() => { $(".toast-message").fadeOut(); }, 3000);
}


function getProcessList(pageLength) {
    // Get current month and year
    let currentDate = new Date();
    let currentMonth = currentDate.getMonth() + 1;  // Month is 0-based, so add 1
    let currentYear = currentDate.getFullYear();

    // Check if the DataTable instance already exists, then destroy it
    if (typeof _dataTable !== 'undefined' && $.fn.DataTable.isDataTable('#salary_process_list_table')) {
        _dataTable.destroy();
    }

    // Initialize the DataTable
    _dataTable = $('#salary_process_list_table').DataTable({
        pageLength: pageLength || 20,
        order: [[1, 'desc'], [2, 'desc']],
        ajax: {
            url: '/api/SalarySettings/GetSalaryProcess',
            dataSrc: 'data',
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error fetching data:', errorThrown);
                alert('Failed to load salary process data. Please try again later.');
            }
        },
        columns: [
            {
                data: null,
                className: 'text-center align-middle',
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                data: 'month',
                orderable: false,
                className: 'text-center'
            },
            {
                data: 'year',
                orderable: false,
                className: 'text-center'
            },
            {
                data: 'employeeType',
                orderable: false,
                className: 'text-center'
            },
            {
                data: 'status',
                orderable: false,
                className: 'text-center',
                render: function (data, type, row) {
                    if (data === "Finally Submitted") {
                        return '<span style="color: red; font-weight: bold;">' + data + '</span>';
                    }
                    return data;
                }
            },
            {
                data: 'salaryProcessingID',
                className: 'text-center',
                render: function (data, type, row) {
                    const monthNames = [
                        'January', 'February', 'March', 'April', 'May', 'June',
                        'July', 'August', 'September', 'October', 'November', 'December'
                    ];
                    let isCurrentMonth = (row.month === monthNames[currentMonth - 1] && row.year === currentYear);
                    return `
                        <button type="button" class="btn btn-success btn-sm" onclick="onReprocessClicked(${data})" 
                            ${isCurrentMonth ? '' : 'disabled'}>
                            Re-Process
                        </button>
                        <button type="button"  id="sendToErpBtn_${row.month}_${row.year}" class="btn btn-primary btn-sm"
                            onclick="confirmSendToERP('${row.month}', ${row.year})">
                            Send To ERP
                        </button>
                       <button type="button" class="btn btn-info btn-sm" onclick="confirmOffprocess(${data})">
                              <i class="fas fa-gift"></i> Final Submit
                            </button>
                        `;
                }
            }
        ],
        lengthMenu: [[10, 20, 50, 100, -1], [10, 20, 50, 100, 'All']],
        lengthChange: true,
        dom: '<"d-flex justify-content-between align-items-center"Bf>tip', // Custom DOM layout
        buttons: [
            'copy', 'excel', 'pdf', 'print' // Export options
        ],
        language: {
            emptyTable: 'No salary process data available', // Custom message for empty table
            loadingRecords: 'Loading...', // Loading message
            zeroRecords: 'No matching records found' // Message for no matches
        },
        columnDefs: [
            { targets: 0, width: '60px' },
            { targets: 1, width: '100px' },
            { targets: 2, width: '120px' },
            { targets: 3, width: '120px' },
            { targets: 4, width: '120px' },
            { targets: 5, width: '180px' }
        ],
        initComplete: function () {
            let api = this.api();
            $('#salary_process_list_table thead th').each(function (index) {
                let title = $(this).text().trim();
                if (index !== 0 && title && title !== 'Action') {
                    $(this).append(`<br><input type="text" class="form-control form-control-sm column-filter" placeholder=" " />`);
                }
            });
            api.columns().every(function () {
                let that = this;
                $('input', this.header()).on('keyup change', function () {
                    if (that.search() !== this.value) {
                        that.search(this.value).draw();
                    }
                });
            });
            $('.dataTables_length select').addClass('form-select form-select-sm');
        }
    });

}
function confirmOffprocess(data) {
    if (confirm("Are you sure you want to Finally Submit (Salary Process)?")) {
        Offprocess(data);
    }
}
function Offprocess(processId) {
    console.log("processId", processId);

    $.ajax({
        url: '/api/SalarySettings/UpdateSalaryProcessMaster',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ SalaryProcessingID: processId }),
        dataType: 'json',
        success: function (responseObject) {
            alert(responseObject.responseMessage);
        },
        error: function (xhr) {
            console.error('Error:', xhr);
            alert('Failed to update salary process Master data.');
        }
    });

    $.ajax({
        url: '/api/Employees/GetEmployeeTypes',
        type: 'GET',
        dataType: 'json',
        success: function (responseObject) {
            const employeeTypes = responseObject.data;
            const $employeeTypeSelect = $('#EmployeeTypeId');
            $employeeTypeSelect.empty();
            $employeeTypeSelect.append(`<option value="0">Select One</option>`);
            $.each(employeeTypes, function (key, item) {
                $employeeTypeSelect.append(`<option value="${item.id}">${item.employeeTypeName}</option>`);
            });
        },
        error: function () {
            alert('Failed to load employee types.');
        }
    });
}
function confirmSendToERP(monthName, year) {
    console.log("MonthName:", monthName, "Year:", year);

    // Safeguard in case inputs are bad
    if (!monthName || !year) {
        alert("Invalid month or year data.");
        return;
    }

    const date = new Date(`${monthName} 1, ${year}`);

    if (isNaN(date)) {
        console.error("Invalid date generated from:", monthName, year);
        alert("Could not parse month/year.");
        return;
    }

    const monthNumber = date.getMonth() + 1;

    const monthId = parseInt(`${year}${monthNumber.toString().padStart(2, '0')}`);
    console.log("Calculated monthId:", monthId);

    if (confirm(`Are you sure you want to send salary process (Expense) to ERP for ${monthName} ${year}?`)) {
        sendToERP(monthId);
    }
}


function sendToERP(monthId) {
    console.log("monthId", monthId);

    // Step 1: Call your internal API
    $.ajax({
        url: `/api/SalarySettings/GetPayrollData?monthId=${monthId}`,
        method: 'GET',
        success: function (response) {
            // Assuming response contains NetPay
            const netPay = response.netPay || 0;

            // Step 2: Call the external API with NetPay as expense
            $.ajax({
                url: 'http://bgfclpay.jabawin.com/Schools/GetExpense',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    expense: netPay
                }),
                success: function () {
                    alert('Successfully sent to ERP and updated external system.');
                    _dataTable.ajax.reload(); // Refresh the table

                    // Step 3: Update the button style and text
                    const btnId = `#sendToErpBtn_${monthName}_${year}`;
                    $(btnId)
                        .removeClass('btn-primary')
                        .addClass('btn-info')
                        .prop('disabled', true)
                        .text('Already Sent');
                },
                error: function (xhr, status, error) {
                    console.error('Failed to update external system:', error);
                    alert('Internal API Success, but external update failed.');
                }
            });
        },
        error: function (xhr, status, error) {
            console.error('Error getting payroll data:', error);
            alert('Failed to send to ERP.');
        }
    });
}




// Custom page length dropdown
$('#page_length_select').on('change', function () {
    let length = parseInt($(this).val(), 10);
    _dataTable.page.len(length).draw();  // Update the page length dynamically and redraw the table
});

function onReprocessClicked(processId) {
    // Show the modal
    $('#process_modal').modal('show');
    console.log("ProcessID", processId)

    // Set the ProcessId field in the modal to the selected process id
    $('#ProcessId').val(processId);

    // You can also update the operation type or button text as needed
    $('#operation_type').val('reprocess');
    $('#process_button').html('Re-Process');
    $('#processModalLabel').html('Re-Process Salary');

    $.ajax({
        url: `/api/SalarySettings/GetSalaryProcessById?id=${processId}`,
        type: 'GET',
        success: function (response) {
            if (response.statusCode === 200 && response.data) {
                $('#ProcessId').val(response.data.id);
               $('#EmployeeTypeId').val(response.data.employeeType);
            } else {
                showToast('Error', response.responseMessage || 'Process not found.', 'error');
                $('#process_modal').modal('hide');
            }
        },
        error: function () {
            showToast('Error', 'An error occurred while fetching Process details.', 'error');
            $('#process_modal').modal('hide');
        }
    });
}


function getMonthId() {
    let month = $('#months').val();
    let year = $('#years').val();

    return (parseInt(year) * 100) + parseInt(month);
}

function loadInitialData() {
    let pageLength = 10;
    getProcessList(pageLength)
    // Call months API
    $.ajax({
        url: '/api/Others/GetMonths',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let months = responseObject.data;
            let currentMonth = new Date().getMonth() + 1;
            $('#months, #f_months').empty();
            $.each(months, function (key, item) {
                let selected = currentMonth === parseInt(item.monthId) ? 'selected' : '';
                $('#months, #f_months').append(`<option value=${item.monthId} ${selected}>${item.monthName}</option>`);
            });
        },
        error: function () {
            alert('Failed to load months.');
        }
    });

    // Call years API
    $.ajax({
        url: '/api/Others/GetYears',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let years = responseObject.data;
            let currentYear = new Date().getFullYear();
            $('#years, #f_years').empty();
            $.each(years, function (key, item) {
                let selected = item == currentYear ? 'selected' : '';
                $('#years, #f_years').append(`<option value=${item} ${selected}>${item}</option>`);
            });
        },
        error: function () {
            alert('Failed to load years.');
        }
    });

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

