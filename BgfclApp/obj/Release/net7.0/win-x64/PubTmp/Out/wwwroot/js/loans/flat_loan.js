let _dataTable = undefined;
$(document).ready(function () {
    getHbLoans();
    initialLoads();

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
            loanTypeId: 9
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
});

function onEditClicked(rowId) {
    $('#edit_cl_modal').modal('show');
    $.ajax({
        url: '/api/Loans/GetComLoanById?id=' + rowId,
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {

            if (responseObject.statusCode == 200) {
                mcLoanData = responseObject.data;
                $('#Id_M').val(mcLoanData.id);
                $('#JobCode_M').val(mcLoanData.jobCode);
                $('#TotalLoanAmount_M').val(mcLoanData.totalLoanAmount);
                $('#RemainingLoanAmount_M').val(mcLoanData.remainingLoanAmount);
                $('#LoanTakenDateString_M').val(mcLoanData.loanTakenDateString);
                $('#InterestRate_M').val(mcLoanData.interestRate);
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
        id: id,
        installmentAmount: installmentAmount,
        isActive: isActive,
        isPaused: isPaused
    }

    $.ajax({
        url: '/api/Loans/UpdateComLoan',
        type: 'PUT',
        async: false,
        data: dataObj
    }).always(function (responseObject) {
        $('.error-item').empty();
        if (responseObject.statusCode == 201) {
            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
            getComLoans();
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

function getHbLoans() {
    var count = 0;

    if (_dataTable != undefined) {
        _dataTable.destroy();
    }


    _dataTable = $('#flat_list_table').DataTable({
        ajax: {
            url: '/api/Loans/GetHbLoansByType?loanTypeId=9',
            dataSrc: 'data'
        },
        columns: [
            { data: 'sl' },
            {
                data: 'id',
                render: (data, type, row) => {
                    return `<button type="button" class="btn btn-primary btn-sm" onclick="onEditClicked('${data}')">edit</button>`;
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
            { data: 'interestInstallmentAmount' },

        ]
    });

}