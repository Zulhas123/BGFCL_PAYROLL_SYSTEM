var jss;
var updatedRow = [];
$(document).ready(function () {

    loadInitialData();
    $('#submit_button').on('click', function () {

        if (jss != undefined) {
            jss.destroy();
            $('#jspreadsheet').empty();
        }

        let _monthId = getMonthId();

        $.ajax({
            url: `/api/SalarySettings/FinalAdjustmentOfficer?monthId=${_monthId}`,
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (responseObject) {
                if (responseObject.statusCode == 200) {
                    $('#update_button').css('display', 'inline-block');
                    $('#excel_export').css('display', 'inline-block');
                    if (responseObject.data.length > 0) {
                        jss = $('#spreadsheet').jspreadsheet({
                            tableOverflow: true,
                            columnSorting: false,
                            freezeColumns: 9,
                            tableWidth: window.innerWidth - 280 + "px",
                            data: responseObject.data,
                            columns: [
                                { type: 'hidden', title: 'Id', width: 100, name: 'id' },
                                { type: 'hidden', title: 'Month Id', width: 100, name: 'monthId' },
                                { type: 'text', title: 'jobCode', width: 100, name: 'jobCode', readOnly: true, },
                                { type: 'text', title: 'Employee Name', width: 100, name: 'employeeName', readOnly: true, css: 'jexcel_readonly' },
                                { type: 'text', title: 'Designation', width: 100, name: 'designationName', readOnly: true, },
                                { type: 'text', title: 'Department', width: 100, name: 'departmentName', readOnly: true, },
                                { type: 'text', title: 'Account', width: 100, name: 'accountNumber', readOnly: true, },
                                { type: 'text', title: 'Bank', width: 100, name: 'bankName', readOnly: true, },
                                { type: 'text', title: 'Branch', width: 100, name: 'bankBranchName', readOnly: true, },
                                { type: 'numeric', title: 'Basic Salary', width: 100, name: 'basicSalary' },
                                { type: 'numeric', title: 'Personal Salary', width: 100, name: 'personalSalary' },
                                { type: 'numeric', title: 'Arrear Salary', width: 100, name: 'arrearSalary' },
                                { type: 'numeric', title: 'Work Days', width: 100, name: 'workDays' },
                                { type: 'numeric', title: 'Like Basic', width: 100, name: 'likeBasic' },
                                { type: 'numeric', title: 'Other Salary', width: 100, name: 'otherSalary' },
                                { type: 'numeric', title: 'Special Benefit', width: 100, name: 'specialBenefit' },
                                { type: 'numeric', title: 'Lunch Allow', width: 100, name: 'lunchAllow' },
                                { type: 'numeric', title: 'Tiffin Allow', width: 100, name: 'tiffinAllow' },
                                { type: 'numeric', title: 'Wash Allow', width: 100, name: 'washAllow' },
                                { type: 'numeric', title: 'HouseRent Allow', width: 100, name: 'houseRentAllow' },
                                { type: 'numeric', title: 'Conveyance', width: 100, name: 'conveyance' },
                                { type: 'numeric', title: 'FM Allow', width: 100, name: 'fmAllow' },
                                { type: 'numeric', title: 'Educational Allow', width: 100, name: 'educationalAllow' },
                                { type: 'numeric', title: 'FieldRisk Allow', width: 100, name: 'fieldRiskAllow' },
                                { type: 'numeric', title: 'Charge Allow', width: 100, name: 'chargeAllow' },
                                { type: 'numeric', title: 'DAid Allow', width: 100, name: 'dAidAllow' },
                                { type: 'numeric', title: 'Deputation Allow', width: 100, name: 'deputationAllow' },
                                { type: 'numeric', title: 'Other Allow', width: 100, name: 'otherAllow' },
                                { type: 'numeric', title: 'Revenue Stamp', width: 100, name: 'revenueStamp' },
                                { type: 'numeric', title: 'Provident Fund', width: 100, name: 'providentFund' },
                                { type: 'numeric', title: 'Pension Officer', width: 100, name: 'pensionOfficer' },
                                { type: 'numeric', title: 'Welfare Fund', width: 100, name: 'welfareFund' },
                                { type: 'numeric', title: 'Officer Club', width: 100, name: 'officerClub' },
                                { type: 'numeric', title: 'Officer Association', width: 100, name: 'officerAssociation' },
                                { type: 'numeric', title: 'Medical Fund', width: 100, name: 'medicalFund' },
                                { type: 'numeric', title: 'TM Bill', width: 100, name: 'tmBill' },
                                { type: 'numeric', title: 'Dormitory', width: 100, name: 'dormitory' },
                                { type: 'numeric', title: 'Hospitalisation', width: 100, name: 'hospitalisation' },
                                { type: 'numeric', title: 'HouseRent Return', width: 100, name: 'houseRentReturn' },
                                { type: 'numeric', title: 'Special Deduction', width: 100, name: 'specialDeduction' },
                                { type: 'numeric', title: 'Fuel Return', width: 100, name: 'fuelReturn' },
                                { type: 'numeric', title: 'Housebuilding Loan', width: 100, name: 'hbLoan' },
                                { type: 'numeric', title: 'Motorcycle Loan', width: 100, name: 'mCylLoan' },
                                { type: 'numeric', title: 'Bicycle Loan', width: 100, name: 'bCylLoan' },
                                { type: 'numeric', title: 'Computer Loan', width: 100, name: 'comLoan' },
                                { type: 'numeric', title: 'Car Loan', width: 100, name: 'carLoan' },
                                { type: 'numeric', title: 'PF Loan', width: 100, name: 'pfLoan' },
                                { type: 'numeric', title: 'WPF Loan', width: 100, name: 'wpfLoan' },
                                { type: 'numeric', title: 'COS Loan', width: 100, name: 'cosLoan' },
                                { type: 'numeric', title: 'Other Loan', width: 100, name: 'otherLoan' },
                                { type: 'numeric', title: 'Advance', width: 100, name: 'advance' },
                                { type: 'numeric', title: 'Other', width: 100, name: 'other' },
                                { type: 'numeric', title: 'Income Tax', width: 100, name: 'incomeTax' },
                                { type: 'numeric', title: 'C.M.E.', width: 100, name: 'cme' }
                            ],
                            onchange: function (instance, cell, x, y, value) {
                                var _newArray = [];
                                var rowId = jss.getValueFromCoords(0, y);
                                var retrivedData = retrivedObject(jss.getRowData(y));
                                if (updatedRow.length == 0) {
                                    updatedRow.push(retrivedData);
                                }
                                else {
                                    for (let x = 0; x < updatedRow.length; x++) {
                                        if (parseInt(updatedRow[x].id) != parseInt(rowId)) {
                                            _newArray.push(updatedRow[x]);
                                        }
                                    }
                                    _newArray.push(retrivedData);
                                    updatedRow = _newArray;
                                }


                            },

                            contextMenu: function (obj, x, y, e) {
                                return [];
                            }
                        });
                        jss.deleteColumn(54, 2);
                    }
                    else {
                        $('#update_button').css('display', 'none');
                        showToast(title = 'Error', message = 'No data found!', toastrType = 'error');
                    }

                }
                if (responseObject.statusCode == 500) {
                    $('#update_button').css('display', 'none');
                    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                }
            },
            error: function (responseObject) {
            }
        });

        //testing
        $.ajax({
            url: `/api/SalarySettings/CheckNetPayOfficer?monthId=${_monthId}`,
            type: 'GET',
            dataType: 'json',
            success: function (responseObject) {
                $('#data-table-body').empty();
                console.log('API Response:', responseObject);
                if (responseObject.statusCode === 200 && responseObject.data.length > 0) {
                    $('#table').css('display', 'inline-block');
                    responseObject.data.forEach(function (item) {
                        const netPayStyle = item.netPay < 0 ? 'color: red;' : '';
                        $('#data-table-body').append(
                            `<tr>
                        <td>${item.jobCode}</td>
                        <td>${item.employeeName}</td>
                        <td style="${netPayStyle}">${item.netPay}</td>
                    </tr>`
                        );
                    });
                } else {
                    //showToast('Error', 'No data found!', 'error');
                }
            },
            error: function (xhr, status, error) {
                console.error('AJAX Error:', status, error);
                showToast('Error', 'Failed to load data.', 'error');
            }
        });
    });

    $('#update_button').on('click', function () {
        if (updatedRow.length > 0) {
            $.ajax({
                url: `/api/SalarySettings/UpdateFinalAdjustmentOfficer`,
                contentType: 'application/json',
                dataType: 'json',
                type: 'POST',
                async: false,
                data: JSON.stringify({ finalAdjustmentOfficers: updatedRow }),
                success: function (responseObject) {
                    if (responseObject.statusCode == 200) {
                        showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    }
                    if (responseObject.statusCode == 500) {
                        showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                    }
                },
                error: function () {

                }

            });
        }
        else {
            showToast(title = 'Error', message = 'No data found!', toastrType = 'error');
        }
    });

});


function retrivedObject(rowData) {
    return {
        id: rowData[0],
        basicSalary: parseFloat(rowData[9]),
        personalSalary: parseFloat(rowData[10]),
        arrearSalary: parseFloat(rowData[11]),
        workDays: parseFloat(rowData[12]),
        likeBasic: parseFloat(rowData[13]),
        otherSalary: parseFloat(rowData[14]),
        specialBenefit: parseFloat(rowData[15]),
        lunchAllow: parseFloat(rowData[16]),
        tiffinAllow: parseFloat(rowData[17]),
        washAllow: parseFloat(rowData[18]),
        houseRentAllow: parseFloat(rowData[19]),
        conveyance: parseFloat(rowData[20]),
        fmAllow: parseFloat(rowData[21]),
        educationalAllow: parseFloat(rowData[22]),
        fieldRiskAllow: parseFloat(rowData[23]),
        chargeAllow: parseFloat(rowData[24]),
        dAidAllow: parseFloat(rowData[25]),
        deputationAllow: parseFloat(rowData[26]),
        otherAllow: parseFloat(rowData[27]),
        revenueStamp: parseFloat(rowData[28]),
        providentFund: parseFloat(rowData[29]),
        pensionOfficer: parseFloat(rowData[30]),
        welfareFund: parseFloat(rowData[31]),
        officerClub: parseFloat(rowData[32]),
        officerAssociation: parseFloat(rowData[33]),
        medicalFund: parseFloat(rowData[34]),
        tmBill: parseFloat(rowData[35]),
        dormitory: parseFloat(rowData[36]),
        hospitalisation: parseFloat(rowData[37]),
        houseRentReturn: parseFloat(rowData[38]),
        specialDeduction: parseFloat(rowData[39]),
        fuelReturn: parseFloat(rowData[40]),
        hbLoan: parseFloat(rowData[41]),
        mCylLoan: parseFloat(rowData[42]),
        bCylLoan: parseFloat(rowData[43]),
        comLoan: parseFloat(rowData[44]),
        carLoan: parseFloat(rowData[45]),
        pfLoan: parseFloat(rowData[46]),
        wpfLoan: parseFloat(rowData[47]),
        cosLoan: parseFloat(rowData[48]),
        otherLoan: parseFloat(rowData[49]),
        advance: parseFloat(rowData[50]),
        other: parseFloat(rowData[51]),
        incomeTax: parseFloat(rowData[52]),
        cme: parseFloat(rowData[53])
    };
}


function getMonthId() {
    let month = $('#months').val();
    let year = $('#years').val();
    let monthId = (parseInt(year) * 100) + parseInt(month);
    $('#excel_export').attr('href', `/SalarySettings/FinalAdjustmentOfficerExcelExport?monthId=${monthId}&month=${month}&year=${year}`);
    return monthId;
}

function loadInitialData() {

    $.ajax({
        url: '/api/Others/GetMonths',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let months = responseObject.data;
            let currentMonth = new Date().getMonth() + 1;
            $('#months').empty();
            $.each(months, function (key, item) {
                if (currentMonth == parseInt(item.monthId)) {
                    $('#months').append(`<option value=${item.monthId} selected>${item.monthName}</option>`);
                }
                else {
                    $('#months').append(`<option value=${item.monthId}>${item.monthName}</option>`);
                }

            });
        },
        error: function (responseObject) {
        }
    });

    $.ajax({
        url: '/api/Others/GetYears',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let years = responseObject.data;
            let currentYear = new Date().getFullYear();
            $('#years').empty();
            $.each(years, function (key, item) {
                if (item == currentYear) {
                    $('#years').append(`<option value=${item} selected>${item}</option>`);
                }
                else {
                    $('#years').append(`<option value=${item}>${item}</option>`);
                }
            });
        },
        error: function (responseObject) {
        }
    });

}
