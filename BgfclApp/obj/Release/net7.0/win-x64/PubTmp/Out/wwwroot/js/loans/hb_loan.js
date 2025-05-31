let _dataTable = undefined;
$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val());
    getHbLoans(pageLength);
    initialLoads();
    $('#filter_select').select2();
    $('#add_button').on('click', function () {
        $('#loanModal').modal('show');
        $('#EmployeeId').val('');
        $('#operation_type').val('create');
        $('#loanModalLabel').html('Add Loan');
    });

    $.ajax({
        url: '/api/Employees/GetAllEmployees',
        method: 'GET',
        success: function (response) {
            var employees = response.data;
            console.log("emp:", employees);
            $('#filter_select').empty();
            $('#filter_select').append($('<option>', { value: '', text: 'All Employee' }));
            employees.forEach(function (employee) {
                $('#filter_select').append(
                    $('<option>', {
                        value: employee.id,   // Assuming employee.id is the correct field for employee ID
                        text: employee.employeeName   // Assuming employee.name is the correct field for employee name
                    })
                );
            });
        },
        error: function (xhr, status, error) {
            console.error('Failed to fetch employees:', error);
        }
    });
    $('#filter_select').on('change', function () {
        var selectedEmployee = $(this).val(); // Get the selected value
        console.log("Selected Employee ID: ", selectedEmployee); // Debug log
        getHbLoans(pageLength,selectedEmployee); // Pass the selected department ID
    });

  
    $('#EmployeeTypeId').on('change', function () {

        let employeeTypeId = parseInt($(this).val());

        $.ajax({
            url: '/api/Loans/GetEmployeesForHbLoan?employeeTypeId=' + employeeTypeId,
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (responseObject) {
                employees = responseObject.data;
                $('#JobCode').empty();
                $('#JobCode').append(`<option value='0'>select one</option>`);
                $.each(employees, function (key, item) {
                    $('#JobCode').append(`<option value=${item.id}>${item.jobCode}</option>`);
                });
            },
            error: function (responseObject) {
            }
        });


    });

    $('#TotalLoanAmount').on('change', function () {
        let totalLoanAmount = $(this).val();
        totalLoanAmount = totalLoanAmount == "" ? 0 : parseFloat(totalLoanAmount);

        let installentNo = $('#InstallmentNo').val();
        installentNo = installentNo == "" ? 0 : parseFloat(installentNo);

        let installmentAmount = totalLoanAmount / installentNo;

        $('#PrincipalInstallmentAmount').val(installmentAmount.toFixed(2));

    });

    $('#InstallmentNo').on('change', function () {
        let totalLoanAmount = $('#TotalLoanAmount').val();
        totalLoanAmount = totalLoanAmount == "" ? 0 : parseFloat(totalLoanAmount);

        let installentNo = $(this).val();
        installentNo = installentNo == "" ? 0 : parseFloat(installentNo);

        let installmentAmount = totalLoanAmount / installentNo;

        $('#PrincipalInstallmentAmount').val(installmentAmount.toFixed(2));
    });

    $('#submit_button').on('click', function () {
        let jobCode = $('#JobCode :selected').text();
        let totalLoanAmount = $('#TotalLoanAmount').val();
        totalLoanAmount = totalLoanAmount == "" ? 0 : parseFloat(totalLoanAmount);
        let loanTakenDate = $('#LoanTakenDateString').val();
        let interestRate = $('#InterestRate').val();
        interestRate = interestRate == "" ? 0 : parseFloat(interestRate);
        let installmentNo = $('#InstallmentNo').val();
        installmentNo = installmentNo == "" ? 0 : parseFloat(installmentNo);
        let principalInstallmentAmount = $('#PrincipalInstallmentAmount').val();
        principalInstallmentAmount = principalInstallmentAmount == "" ? 0 : parseFloat(principalInstallmentAmount);
        let interestInstallmentNo = $('#InterestInstallmentNo').val();
        interestInstallmentNo = interestInstallmentNo == "" ? 0 : parseFloat(interestInstallmentNo);
        let interestInstallmentAmount = $('#InterestInstallmentAmount').val();
        interestInstallmentAmount = interestInstallmentAmount == "" ? 0 : parseFloat(interestInstallmentAmount);

        let dataObj = {
            jobCode: jobCode,
            totalLoanAmount: totalLoanAmount,
            loanTakenDateString: loanTakenDate,
            interestRate: interestRate,
            installmentNo: installmentNo,
            principalInstallmentAmount: principalInstallmentAmount,
            interestInstallmentNo: interestInstallmentNo,
            interestInstallmentAmount: interestInstallmentAmount,
            loanTypeId:1
        };

        $.ajax({
            url: '/api/Loans/CreateHbLoan',
            type: 'POST',
            async: false,
            data: dataObj
        }).always(function (responseObject) {
            $('.error-item').empty();
            if (responseObject.statusCode == 201) {
                $('#TotalLoanAmount').val('');
                $('#LoanTakenDateString').val('');
                $('#InterestRate').val('');
                $('#InstallmentNo').val('');
                $('#PrincipalInstallmentAmount').val('');
                $('#JobCode').empty();
                $('#InterestInstallmentNo').val('');
                $('#InterestInstallmentAmount').val('');
                initialLoads();

                showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                $('#loanModal').modal('hide');
                getHbLoans();
            }
            if (responseObject.statusCode == 400 || responseObject.statusCode == 409) {
                for (let error in responseObject.errors) {
                    $(`#${error}`).empty();
                    $(`#${error}`).append(responseObject.errors[error]);
                }
                showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
            }
            if (responseObject.statusCode == 500) {
                showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
            }
        });
    });

    $('#cancel').on('click', () => {
        $('#re_schedule_hbl_modal').modal('hide');
        location.reload();
    });
    $('#page_length_select').on('change', function () {
        pageLength = parseInt($(this).val());
        getHbLoans(pageLength);
    });


    $('#RescheduledLoanAmount_R').on('change', function () {
        let remainingLoanAmount = $('#RemainingLoanAmount_R').val();
        remainingLoanAmount = remainingLoanAmount == "" ? 0 : parseFloat(remainingLoanAmount);

        let rescheduledLoanAmount = $(this).val();
        rescheduledLoanAmount = rescheduledLoanAmount == "" ? 0 : parseFloat(rescheduledLoanAmount);

        let totalAmount = remainingLoanAmount + rescheduledLoanAmount;

        $('#TotalLoanAmount_R').val(totalAmount);
    });

    $('#InstallmentNo_R').on('change', function () {
        let totalLoanAmount = $('#TotalLoanAmount_R').val();
        totalLoanAmount = totalLoanAmount == "" ? 0 : parseFloat(totalLoanAmount);

        let installmentNo = $(this).val();
        installmentNo = installmentNo == "" ? 0 : parseFloat(installmentNo);

        let principalInstallmentAmount = totalLoanAmount / installmentNo;

        $('#PrincipalInstallmentAmount_R').val(principalInstallmentAmount.toFixed(2));
    });

});

function onEditClicked(rowId) {
    $('#edit_hbl_modal').modal('show');
    $.ajax({
        url: '/api/Loans/GetHbLoanById?id=' + rowId,
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {

            if (responseObject.statusCode == 200) {
                hbLoanData = responseObject.data;
                $('#Id_M').val(hbLoanData.id);
                $('#JobCode_M').val(hbLoanData.jobCode);
                $('#TotalLoanAmount_M').val(hbLoanData.totalLoanAmount);
                $('#LoanTakenDateString_M').val(hbLoanData.loanTakenDateString);
                $('#InterestRate_M').val(hbLoanData.interestRate);
                $('#InstallmentNo_M').val(hbLoanData.installmentNo);
                $('#PrincipalInstallmentAmount_M').val(hbLoanData.principalInstallmentAmount);
                $('#InterestInstallmentNo_M').val(hbLoanData.interestInstallmentNo);
                $('#RemainingInterestInstallmentNo_M').val(hbLoanData.remainingInterestInstallmentNo);
                $('#InterestInstallmentAmount_M').val(hbLoanData.interestInstallmentAmount);
                $('#isactive_checkbox').prop('checked', hbLoanData.isActive);
                $('#pause_checkbox').prop('checked', hbLoanData.isPaused);
            }

        },
        error: function (responseObject) {
        }
    });
}

function onEditConfirmed() {
    let id = $('#Id_M').val();
    let principalInstallmentAmount = $('#PrincipalInstallmentAmount_M').val();
    principalInstallmentAmount = principalInstallmentAmount == '' ? 0 : parseFloat(principalInstallmentAmount);
    let interestInstallmentAmount = $('#InterestInstallmentAmount_M').val();
    interestInstallmentAmount = interestInstallmentAmount == '' ? 0 : parseFloat(interestInstallmentAmount);
    let interestInstallmentNo = $('#InterestInstallmentNo_M').val();
    interestInstallmentNo = interestInstallmentNo == '' ? 0 : parseFloat(interestInstallmentNo);
    let remainingInterestInstallmentNo = $('#RemainingInterestInstallmentNo_M').val();
    remainingInterestInstallmentNo = remainingInterestInstallmentNo == '' ? 0 : parseFloat(remainingInterestInstallmentNo);
    let isActive = $('#isactive_checkbox').is(':checked');
    let isPaused = $('#pause_checkbox').is(':checked');

    let dataObj = {
        id: id,
        principalInstallmentAmount: principalInstallmentAmount,
        interestInstallmentAmount: interestInstallmentAmount,
        interestInstallmentNo: interestInstallmentNo,
        remainingInterestInstallmentNo: remainingInterestInstallmentNo,
        isActive: isActive,
        isPaused: isPaused
    }

    $.ajax({
        url: '/api/Loans/UpdateHbLoan',
        type: 'PUT',
        async: false,
        data: dataObj
    }).always(function (responseObject) {
        $('.error-item').empty();
        if (responseObject.statusCode == 201) {
            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
            $('#edit_hbl_modal').modal('hide');
            getHbLoans();
        }
        if (responseObject.statusCode == 400) {
            for (let error in responseObject.errors) {
                $(`#${error}`).empty();
                $(`#${error}`).append(responseObject.errors[error]);
            }
            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
        }
        if (responseObject.statusCode == 500) {
            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
        }
    });


}
function onRescheduleClicked(rowId) {
    $('#re_schedule_hbl_modal').modal('show');

    // clear inputs
    $('#Id_R').val('');
    $('#JobCode_R').val('');
    $('#RemainingLoanAmount_R').val('');
    $('#RescheduledLoanAmount_R').val('');
    $('#TotalLoanAmount_R').val('');
    $('#RemainingInterest_R').val('');
    $('#InstallmentNo_R').val('');
    $('#PrincipalInstallmentAmount_R').val('');
    $('#InterestInstallmentNo_R').val('');
    $('#LoanTakenDateString_R').val('');
    $('#InterestRate_R').val('');

    $.ajax({
        url: '/api/Loans/GetHbLoanById?id=' + rowId,
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {

            if (responseObject.statusCode == 200) {
                hbLoanData = responseObject.data;
                $('#Id_R').val(hbLoanData.id);
                $('#JobCode_R').val(hbLoanData.jobCode);
                $('#RemainingLoanAmount_R').val(hbLoanData.remainingLoanAmount);
                $('#RemainingInterest_R').val(hbLoanData.remainingInterest);
            }

        },
        error: function (responseObject) {
        }
    });
}
function onRescheduleConfirmed() {
    let id = $('#Id_R').val();
    let remainingLoanAmount = $('#RemainingLoanAmount_R').val();
    remainingLoanAmount = remainingLoanAmount == '' ? 0 : parseFloat(remainingLoanAmount);

    let rescheduledLoanAmount = $('#RescheduledLoanAmount_R').val();
    rescheduledLoanAmount = rescheduledLoanAmount == '' ? 0 : parseFloat(rescheduledLoanAmount);

    let totalLoanAmount = $('#TotalLoanAmount_R').val();
    totalLoanAmount = totalLoanAmount == '' ? 0 : parseFloat(totalLoanAmount);

    let remainingInterest = $('#RemainingInterest_R').val();
    remainingInterest = remainingInterest == '' ? 0 : parseFloat(remainingInterest);

    let installmentNo = $('#InstallmentNo_R').val();
    installmentNo = installmentNo == '' ? 0 : parseFloat(installmentNo);

    let principalInstallmentAmount = $('#PrincipalInstallmentAmount_R').val();
    principalInstallmentAmount = principalInstallmentAmount == '' ? 0 : parseFloat(principalInstallmentAmount);

    let interestInstallmentNo = $('#InterestInstallmentNo_R').val();
    interestInstallmentNo = interestInstallmentNo == '' ? 0 : parseFloat(interestInstallmentNo);

    let loanTakenDateString = $('#LoanTakenDateString_R').val();

    let interestRate = $('#InterestRate_R').val();
    interestRate = interestRate == '' ? 0 : parseFloat(interestRate);

    let dataObj = {
        hBLoanId: id,
        remainingLoanAmount: remainingLoanAmount,
        rescheduledLoanAmount: rescheduledLoanAmount,
        totalLoanAmount: totalLoanAmount,
        remainingInterest: remainingInterest,
        installmentNo: installmentNo,
        principalInstallmentAmount: principalInstallmentAmount,
        interestInstallmentNo: interestInstallmentNo,
        loanTakenDateString: loanTakenDateString,
        interestRate: interestRate
    }

    $.ajax({
        url: '/api/Loans/CreateHBLReschedule',
        type: 'PUT',
        async: false,
        data: dataObj
    }).always(function (responseObject) {
       // $('.error-item').empty();
        if (responseObject.statusCode == 201) {
            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
            $('#re_schedule_hbl_modal').modal('hide');
            getHbLoans();
        }
        //if (responseObject.statusCode == 400) {
        //    for (let error in responseObject.errors) {
        //        $(`#${error}`).empty();
        //        $(`#${error}`).append(responseObject.errors[error]);
        //    }
        //    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
        //}
        if (responseObject.statusCode == 500) {
            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
        }
    });


}
function initialLoads() {
    $.ajax({
        url: '/api/Employees/GetEmployeeTypes',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            employeeTypes = responseObject.data;
            $('#EmployeeTypeId').empty();
            $('#EmployeeTypeId').append('<option value="0">select one</option>');
            $.each(employeeTypes, function (key, item) {
                if (item.id != 3 && item.id != 4) {
                    $('#EmployeeTypeId').append(`<option value=${item.id}>${item.employeeTypeName}</option>`);
                }

            });
        },
        error: function (responseObject) {
        }
    });
}

function getHbLoans(pageLength,selectedEmployee) {
    var count = 0;

    if (_dataTable != undefined) {
        _dataTable.destroy();
    }
    console.log("selectedEmployee", selectedEmployee)
    // Determine the URL based on the selectedEmployee
    const url = selectedEmployee && selectedEmployee !== '0'
        ? `/api/Loans/GetHbLoansByEmployeeId?id=${selectedEmployee}`
        : '/api/Loans/GetHbLoansByType?loanTypeId=1';

    // Initialize the DataTable
    _dataTable = $('#hbl_list_table').DataTable({
        pageLength: pageLength,
        scrollX: true, // Enables horizontal scrolling
        responsive: true,
        ajax: {
            url: url, // Use the dynamic URL based on selectedEmployee
            dataSrc: 'data'
        },
        columns: [
            { data: 'sl' },
            {
                data: 'id',
                render: (data, type, row) => {
                    return `<button type="button" class="btn btn-warning btn-sm" onclick="onEditClicked('${data}')">Edit</button>`;
                }
            },
            {
                data: 'id',
                render: (data, type, row) => {
                    return `<button type="button" class="btn btn-success btn-sm" onclick="onRescheduleClicked('${data}')">Reschedule</button>`;
                }
            },
            { data: 'jobCode' },
            { data: 'employeeName' },
            { data: 'loanTakenDate' },
            { data: 'interestRate' },
            { data: 'totalLoanAmount' },
            { data: 'remainingLoanAmount' },
            { data: 'installmentNo' },
            { data: 'remainingInstallmentNo' },
            { data: 'principalInstallmentAmount' },
            { data: 'totalInterest' },
            { data: 'remainingInterest' },
            { data: 'interestInstallmentNo' },
            { data: 'remainingInterestInstallmentNo' },
            { data: 'interestInstallmentAmount' }
        ],
        dom: '<"d-flex justify-content-between align-items-center"Bf>tip', // Custom layout for buttons
        buttons: ['copy', 'excel', 'pdf', 'print'], // Export buttons
        paging: true // Enable pagination
    });
}

function onClear() {
    // clear inputs
    $('#RescheduledLoanAmount_R').val('');
    $('#TotalLoanAmount_R').val('');
    $('#InstallmentNo_R').val('');
    $('#PrincipalInstallmentAmount_R').val('');
    $('#InterestInstallmentNo_R').val('');
    $('#LoanTakenDateString_R').val('');
    $('#InterestRate_R').val('');
}
