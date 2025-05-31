let _dataTable = undefined;
$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val());
    getCarLoans(pageLength);
    $('#filter_select').select2();
    initialLoads();
    $('#JobCode').select2({
        dropdownParent: ("#carLoanModalLabel")
    });

    $('#add_button').on('click', function () {
        $('#car_loan_modal').modal('show');
        //$('#LoanId').val('');carLoanModalLabel
        //$('#operation_type').val('create');
        //$('#submit_button').html('Create');
        $('#').html('Add Car Loan');
    });

    $('#filter_select').on('change', function () {
        var selectedcarloan = $(this).val(); // Get the selected value
        getCarLoans(pageLength, selectedcarloan); // Pass the selected department ID
    });
    $('#submit_button').on('click', function () {
        console.log("Click")
        let jobCode = $('#JobCode :selected').text();
        let totalLoanAmount = $('#TotalLoanAmount').val();
        totalLoanAmount = totalLoanAmount == "" ? 0 : parseFloat(totalLoanAmount);
        let loanTakenDate = $('#LoanTakenDateString').val();
        let interestRate = $('#InterestRate').val();
        interestRate = interestRate == "" ? 0 : parseFloat(interestRate);
        let installmentNo = $('#InstallmentNo').val();
        installmentNo = installmentNo == "" ? 0 : parseFloat(installmentNo);
        let depreciationAmount = $('#DepreciationAmount').val();
        depreciationAmount = depreciationAmount == "" ? 0 : parseFloat(depreciationAmount);
        let actualAmount = $('#ActualAmount').val();
        actualAmount = actualAmount == "" ? 0 : parseFloat(actualAmount);
        let emiStartMonthString = $('#EmiStartMonthString').val();

        let dataObj = {
            jobCode: jobCode,
            totalLoanAmount: totalLoanAmount,
            loanTakenDateString: loanTakenDate,
            interestRate: interestRate,
            installmentNo: installmentNo,
            depreciationAmount: depreciationAmount,
            actualAmount: actualAmount,
            emiStartMonthString: emiStartMonthString
        };

        $.ajax({
            url: '/api/Loans/CreateCarLoan',
            type: 'POST',
            async: false,
            data: dataObj
        }).always(function (responseObject) {
            $('#car_loan_modal').modal('hide');
            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
            $('.error-item').empty();
            if (responseObject.statusCode == 201) {
                $('#TotalLoanAmount').val('');
                $('#LoanTakenDateString').val('');
                $('#InterestRate').val('');
                $('#InstallmentNo').val('');
                $('#DepreciationAmount').val('');
                $('#ActualAmount').val('');

                initialLoads();
                $('#car_loan_modal').modal('hide');
                showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                getCarLoans();
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
        $('#carLoanForm')[0].reset();
        $('#TotalLoanAmount').text('');
        $('#DepreciationAmount').text('');
        $('#ActualAmount').text('');
        $('#LoanTakenDateString').text('');
        $('#InterestRate').text('');
        $('#InstallmentNo').text('');
        $('#EmiStartMonthString').text('');
        $('#car_loan_modal').modal('hide');
    });
    $('#page_length_select').on('change', function () {
        pageLength = parseInt($(this).val());
        getCarLoans(pageLength);
    });
});







function onInstallmentsClicked(rowId) {

    $.ajax({
        url: '/api/Loans/GetCarLoanInstallments?loanId=' + rowId,
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {

            if (responseObject.statusCode == 200) {
                $("#generate_installment_pdf").prop("href", `/Reports/CarInstallments?loanId=${rowId}`);
                const carLoanData = responseObject.data.carLoan;
                const employeeData = responseObject.data.employee;
                const carLoanInstallmentsData = responseObject.data.installments;

                // Extract relevant info
                const jobCode = carLoanData.jobCode;
                const employeeName = employeeData.employeeName;
                const totalAmount = carLoanInstallmentsData.reduce((sum, installment) => sum + installment.principalAmount, 0);

                // Calculate total paid and unpaid amounts
                let paidAmount = 0;
                let unpaidAmount = 0;

                carLoanInstallmentsData.forEach(installment => {
                    if (installment.isPaid) {
                        paidAmount += installment.principalAmount;
                    } else {
                        unpaidAmount += installment.principalAmount;
                    }
                });

                // Update the #headerInfo div
                $('#headerInfo').html(`
                <div style="display: flex; justify-content: space-between; align-items: center; padding: 10px; border: 1px solid #ccc; background-color: #f9f9f9;">
                    <span style="text-align: left;"><strong>Job Code:</strong> ${jobCode}</span>
                    <span style="text-align: center;"><strong>Employee Name:</strong> ${employeeName}</span>
                    <span style="text-align: right;"><strong>Total Loan Amount:</strong> ${totalAmount.toLocaleString()}</span>
                </div>
               `);

                // Update the #footerInfo div
                $('#footerInfo').html(`
                    <div style="display: flex; justify-content: space-between; align-items: center; padding: 10px; border: 1px solid #ccc; background-color: #f9f9f9;">
                        <span style="text-align: left;"><strong>Total Paid:</strong> ${paidAmount.toFixed(2)}</span>
                        <span style="text-align: right;"><strong>Total Unpaid:</strong> ${unpaidAmount.toFixed(2)}</span>
                    </div>
                `);


                $('#car_loan_installments_table tbody').empty();
                let count = 1;
                for (let x = 0; x < carLoanInstallmentsData.length; x++) {
                    const installment = carLoanInstallmentsData[x];
                    const status = installment.isPaid ? 'Paid' : 'Unpaid';

                    $('#car_loan_installments_table tbody').append(`
                                <tr>
                                    <td style='text-align:center;'>${count}</td>
                                    <td style='text-align:center;'>${installment.monthId}</td>
                                    <td style='text-align:right;'>${installment.principalAmount.toFixed(2)}</td>
                                    <td style='text-align:right;'>${installment.interestAmount.toFixed(2)}</td>
                                    <td style='text-align:right;'>${installment.totalPayment.toFixed(2)}</td>
                                    <td style='text-align:right;'>${installment.remainingBalance.toFixed(2)}</td>
                                    <td style='text-align:right;'>${installment.depreciationAmount.toFixed(2)}</td>
                                    <td style='text-align:center;'>${status}</td>
                                </tr>
                    `);

                    count++;
                }
            }


        },
        error: function (responseObject) {
        }
    });
    $('#car_loan_installments_modal').modal('show');
}

function onDepreciationsClicked(rowId) {

    $.ajax({
        url: '/api/Loans/GetCarLoanDepreciationInstallments?loanId=' + rowId,
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {

            if (responseObject.statusCode == 200) {
                $("#generate_depriciate_pdf").prop("href", `/Reports/CarDepreciationInstallments?loanId=${rowId}`);
                console.log("obj", responseObject)
                //carLoanInstallmentsData = responseObject.data;

                const carLoanData = responseObject.data.carLoan;
                const employeeData = responseObject.data.employee;
                const carLoandepriciationData = responseObject.data.depreciationInstallments;

                // Extract relevant info
                const jobCode = carLoanData.jobCode;
                const employeeName = employeeData.employeeName;
                const totalAmount = carLoandepriciationData.reduce((sum, installment) => sum + installment.depreciationAmount, 0);

                // Calculate total paid and unpaid amounts
                let paidAmount = 0;
                let unpaidAmount = 0;

                carLoandepriciationData.forEach(depreciation => {
                    if (depreciation.isPaid) {
                        paidAmount += depreciation.depreciationAmount;
                    } else {
                        unpaidAmount += depreciation.depreciationAmount;
                    }
                });

                // Update the #headerInfo div
                $('#d_headerInfo').html(`
                <div style="display: flex; justify-content: space-between; align-items: center; padding: 10px; border: 1px solid #ccc; background-color: #f9f9f9;">
                    <span style="text-align: left;"><strong>Job Code:</strong> ${jobCode}</span>
                    <span style="text-align: center;"><strong>Employee Name:</strong> ${employeeName}</span>
                    <span style="text-align: right;"><strong>Total Depreciation Amount:</strong> ${totalAmount.toLocaleString()}</span>
                </div>
               `);

                // Update the #footerInfo div
                $('#d_footerInfo').html(`
                    <div style="display: flex; justify-content: space-between; align-items: center; padding: 10px; border: 1px solid #ccc; background-color: #f9f9f9;">
                        <span style="text-align: left;"><strong>Total Paid:</strong> ${paidAmount.toFixed(2)}</span>
                        <span style="text-align: right;"><strong>Total Unpaid:</strong> ${unpaidAmount.toFixed(2)}</span>
                    </div>
                `);





                $('#car_loan_depreciation_installments_table tbody').empty();
                let count = 1;
                for (let x = 0; x < carLoandepriciationData.length; x++) {
                    let status = '';
                    if (carLoandepriciationData[x].isPaid) {
                        status = 'Paid';
                    }
                    else {
                        status = 'Unpaid';
                    }

                    $('#car_loan_depreciation_installments_table tbody').append(`
                    <tr>
                        <td style='text-align:center;'>${count}</td>
                        <td style='text-align:center;'>${carLoandepriciationData[x].monthId}</td>
                        <td style='text-align:right;'>${carLoandepriciationData[x].depreciationAmount}</td>
                        <td style='text-align:center;'>${status}</td>
                    </tr>
                    `);

                    count++;
                }

            }

        },
        error: function (responseObject) {
        }
    });
    $('#car_loan_depreciation_installments_modal').modal('show');
}





//function onEditConfirmed() {
//    let id = $('#Id_M').val();
//    let installmentAmount = $('#InstallmentAmount_M').val();
//    installmentAmount = installmentAmount == '' ? 0 : parseFloat(installmentAmount)
//    let isActive = $('#isactive_checkbox').is(':checked');
//    let isPaused = $('#pause_checkbox').is(':checked');

//    let dataObj = {
//        id: id,
//        installmentAmount: installmentAmount,
//        isActive: isActive,
//        isPaused: isPaused
//    }

//    $.ajax({
//        url: '/api/Loans/UpdateComLoan',
//        type: 'PUT',
//        async: false,
//        data: dataObj
//    }).always(function (responseObject) {
//        $('.error-item').empty();
//        if (responseObject.statusCode == 201) {
//            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
//            getComLoans();
//        }
//        if (responseObject.statusCode == 400) {
//            for (let error in responseObject.errors) {
//                $(`#${error}`).empty();
//                $(`#${error}`).append(responseObject.errors[error]);
//            }
//            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
//        }
//        if (responseObject.statusCode == 500) {
//            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
//        }
//    });


//}


function initialLoads() {
    $.ajax({
        url: '/api/Loans/GetEmployeesForCarLoan?employeeTypeId=1',
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

}

function getCarLoans(pageLength) {
    var count = 0;

    if (_dataTable != undefined) {
        _dataTable.destroy();
    }

    _dataTable = $('#car_loan_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url: '/api/Loans/GetCarLoans',
            dataSrc: 'data'
        },
        columns: [
            { data: 'sl' },
            {
                data: 'id',
                render: (data, type, row) => {
                    return `<button type="button" class="btn btn-primary btn-sm" onclick="onInstallmentsClicked('${data}')">installments</button>`;
                }
            },
            {
                data: 'id',
                render: (data, type, row) => {
                    return `<button type="button" class="btn btn-primary btn-sm" onclick="onDepreciationsClicked('${data}')">depreciations</button>`;
                }
            },
            { data: 'jobCode' },
            { data: 'employeeName' },
            { data: 'loanTakenDate' },
            { data: 'interestRate' },
            { data: 'totalLoanAmount' },
            { data: 'depreciationAmount' },
            { data: 'remainingDepreciationAmount' },
            { data: 'actualAmount' },
            { data: 'remainingActualAmount' },
            { data: 'installmentNo' },
            { data: 'remainingInstallmentNo' }
        ],
        dom: '<"d-flex justify-content-between align-items-center"Bf>tip',
        buttons: [
            'copy', 'excel', 'pdf', 'print'
        ],
        scrollX: true, // Enable horizontal scrolling
        autoWidth: false // Prevent automatic column width adjustments
    });
}
