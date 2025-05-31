let _dataTable = undefined;
$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val());
    getMcLoans(pageLength);
    $('#filter_select').select2();
    $('#JobCode').select2({
        dropdownParent: ("#motor_loan_modal")
    });
    initialLoads();
    $('#add_button').on('click', function () {
        $('#motor_loan_modal').modal('show');
        $('#').html('Add Motor Loan');
    });
    $('#filter_select').on('change', function () {
        var selectedmotorloan = $(this).val(); // Get the selected value
        getMcLoans(pageLength, selectedmotorloan); // Pass the selected department ID
    });
    $('#EmployeeTypeId').on('change', function () {

        let employeeTypeId = parseInt($(this).val());

        $.ajax({
            url: '/api/Loans/GetEmployeesForMcLoan?employeeTypeId=' + employeeTypeId,
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

        $('#InstallmentAmount').val(installmentAmount.toFixed(2));

    });

    $('#InstallmentNo').on('change', function () {
        let totalLoanAmount = $('#TotalLoanAmount').val();
        totalLoanAmount = totalLoanAmount == "" ? 0 : parseFloat(totalLoanAmount);

        let installentNo = $(this).val();
        installentNo = installentNo == "" ? 0 : parseFloat(installentNo);

        let installmentAmount = totalLoanAmount / installentNo;

        $('#InstallmentAmount').val(installmentAmount.toFixed(2));
    });

    $('#submit_button').on('click', function () {
        console.log("Click")
        let jobCode = $('#JobCode :selected').text();
        let totalLoanAmount = $('#TotalLoanAmount').val();
        totalLoanAmount = totalLoanAmount == "" ? 0 : parseFloat(totalLoanAmount);
        let loanTakenDate = $('#LoanTakenDateString').val();
        let installmentNo = $('#InstallmentNo').val();
        installmentNo = installmentNo == "" ? 0 : parseFloat(installmentNo);
        let installmentAmount = $('#InstallmentAmount').val();
        installmentAmount = installmentAmount == "" ? 0 : parseFloat(installmentAmount);

        let dataObj = {
            jobCode: jobCode,
            totalLoanAmount: totalLoanAmount,
            loanTakenDateString: loanTakenDate,
            installmentNo: installmentNo,
            installmentAmount: installmentAmount
        };

        $.ajax({
            url: '/api/Loans/CreateMcLoan',
            type: 'POST',
            async: false,
            data: dataObj
        }).always(function (responseObject) {
            $('.error-item').empty();
            if (responseObject.statusCode == 201) {
                $('#TotalLoanAmount').val('');
                $('#LoanTakenDateString').val('');
                $('#InstallmentNo').val('');
                $('#InstallmentAmount').val('');
                $('#JobCode').empty();
                initialLoads();
                $('#motor_loan_modal').modal('hide');
                showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                getMcLoans();
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
    $('#cancel_button').on('click', () => {
        /* $('#JobCode').empty();*/
        $('#motorLoanForm')[0].reset();
        $('#TotalLoanAmount').text('');;
        $('#LoanTakenDateString').text('');
        $('#InstallmentNo').text('');
        $('#InstallmentAmount').text('');
        $('#motor_loan_modal').modal('hide');
    });
});

function onEditClicked(rowId) {
    $('#edit_mcl_modal').modal('show');
    $.ajax({
        url: '/api/Loans/GetMcLoanById?id=' + rowId,
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {

            if (responseObject.statusCode==200) {
                mcLoanData = responseObject.data;
                $('#Id_M').val(mcLoanData.id);
                $('#JobCode_M').val(mcLoanData.jobCode);
                $('#TotalLoanAmount_M').val(mcLoanData.totalLoanAmount);
                $('#RemainingLoanAmount_M').val(mcLoanData.remainingLoanAmount);
                $('#LoanTakenDateString_M').val(mcLoanData.loanTakenDateString);
                $('#InstallmentNo_M').val(mcLoanData.installmentNo);
                $('#RemainingInstallmentNo_M').val(mcLoanData.remainingInstallmentNo);
                $('#InstallmentAmount_M').val(mcLoanData.installmentAmount);
                $('#isactive_checkbox').prop('checked', mcLoanData.isActive);
                $('#pause_checkbox').prop('checked', mcLoanData.isPaused);
            }
           
        },
        error: function (responseObject) {
        }
    });
}

function onEditConfirmed() {
    let id = $('#Id_M').val();
    let installmentAmount = $('#InstallmentAmount_M').val();
    installmentAmount = installmentAmount == '' ? 0 : parseFloat(installmentAmount)
    let isActive = $('#isactive_checkbox').is(':checked');
    let isPaused = $('#pause_checkbox').is(':checked');

    let dataObj = {
        id:id,
        installmentAmount: installmentAmount,
        isActive: isActive,
        isPaused: isPaused
    }

    $.ajax({
        url: '/api/Loans/UpdateMcLoan',
        type: 'PUT',
        async: false,
        data: dataObj
    }).always(function (responseObject) {
        $('.error-item').empty();
        $('#motor_loan_modal').modal('hide');
        if (responseObject.statusCode == 201) {
            $('#motor_loan_modal').modal('hide');
            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
            getMcLoans();
        }
        if (responseObject.statusCode == 400) {
            for (let error in responseObject.errors) {
                $(`#${error}`).empty();
                $(`#${error}`).append(responseObject.errors[error]);
            }
            $('#motor_loan_modal').modal('hide');
            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
        }
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

function getMcLoans(pageLength) {
    var count = 0;

    if (_dataTable != undefined) {
        _dataTable.destroy();
    }


    _dataTable = $('#mcl_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url: '/api/Loans/GetMcLoans',
            dataSrc: 'data'
        },
        columns: [
            { data: 'sl' },
            {
                data: 'id',
                render: (data, type, row) => {
                    return `<button type="button" class="btn btn-success btn-sm" onclick="onEditClicked('${data}')">Edit</button>`;
                }
            },
            { data: 'jobCode' },
            { data: 'employeeName' },
            { data: 'loanTakenDate' },
            { data: 'totalLoanAmount' },
            { data: 'remainingLoanAmount' },
            { data: 'installmentNo' },
            { data: 'remainingInstallmentNo' },
            { data: 'installmentAmount' }

        ],
        dom: '<"d-flex justify-content-between align-items-center"Bf>tip',
        buttons: [
            'copy', 'excel', 'pdf', 'print'
        ],
        scrollX: true, // Enable horizontal scrolling
        autoWidth: false // Prevent automatic column width adjustments
    });

}