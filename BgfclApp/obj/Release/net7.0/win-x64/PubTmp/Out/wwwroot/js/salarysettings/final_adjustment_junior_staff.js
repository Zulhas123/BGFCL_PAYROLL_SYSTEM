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
            url: `/api/SalarySettings/FinalAdjustmentJuniorStaff?monthId=${_monthId}`,
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
                                { type: 'text', title: 'employee Name', width: 100, name: 'employeeName', readOnly: true, },
                                { type: 'text', title: 'Designation', width: 100, name: 'designationName', readOnly: true, },
                                { type: 'text', title: 'Department', width: 100, name: 'departmentName', readOnly: true, },
                                { type: 'text', title: 'Account', width: 100, name: 'accountNumber', readOnly: true, },
                                { type: 'text', title: 'Bank', width: 100, name: 'bankName', readOnly: true, },
                                { type: 'text', title: 'Branch', width: 100, name: 'bankBranchName', readOnly: true, },
                                { type: 'decimal', title: 'Basic Salary', width: 100,  name: 'basicSalary' },
                                { type: 'decimal', title: 'Personal Salary', width: 100,   name: 'personalSalary' },
                                { type: 'decimal', title: 'Convenience Allow', width: 100,   name: 'convenienceAllow' },
                                { type: 'decimal', title: 'Arrear Salary', width: 100,   name: 'arrearSalary' },
                                { type: 'decimal', title: 'Work Days', width: 100,  name: 'workDays' },
                                { type: 'decimal', title: 'Number of Shift', width: 100,   name: 'numberOfShift' },
                                { type: 'decimal', title: 'Other Salary', width: 100,   name: 'otherSalary' },
                                { type: 'decimal', title: 'Special Benefit', width: 100,  name: 'specialBenefit' },
                                { type: 'decimal', title: 'Lunch Allow', width: 100,   name: 'lunchAllow' },
                                { type: 'decimal', title: 'Tiffin Allow', width: 100,   name: 'tiffinAllow' },
                                { type: 'decimal', title: 'Shift Allow', width: 100,  name: 'shiftAllow' },
                                { type: 'decimal', title: 'HouseRent Allow', width: 100, name: 'houseRentAllow' },
                                { type: 'decimal', title: 'FamilyMedical Allow', width: 100,   name: 'familyMedicalAllow' },
                                { type: 'decimal', title: 'Education Allowance', width: 100,   name: 'educationAllowance' },
                                { type: 'decimal', title: 'Field Allow', width: 100,   name: 'fieldAllow' },
                                { type: 'decimal', title: 'OT Single', width: 100,  name: 'otSingle' },
                                { type: 'decimal', title: 'OT Double', width: 100,   name: 'otDouble' },
                                { type: 'decimal', title: 'OT Allow', width: 100,   name: 'otAllow' },
                                { type: 'decimal', title: 'Fuel Allow', width: 100,  name: 'fuelAllow' },
                                { type: 'decimal', title: 'Utility Allow', width: 100,   name: 'utilityAllow' },
                                { type: 'decimal', title: 'Other Allow', width: 100,  name: 'otherAllow' },
                                { type: 'decimal', title: 'Revenue Stamp', width: 100,   name: 'revenueStamp' },
                                { type: 'decimal', title: 'provident Fund', width: 100,   name: 'providentFund' },
                                { type: 'decimal', title: 'Welfare Fund', width: 100,  name: 'welfareFund' },
                                { type: 'decimal', title: 'Employee Club', width: 100,   name: 'employeeClub' },
                                { type: 'decimal', title: 'Employee Union', width: 100,   name: 'employeeUnion' },
                                { type: 'decimal', title: 'Dormitory', width: 100,   name: 'dormitory' },
                                { type: 'decimal', title: 'Hospital Deduction', width: 100,  name: 'hospitalDeduction' },
                                { type: 'decimal', title: 'Special Deduction', width: 100,   name: 'specialDeduction' },
                                { type: 'decimal', title: 'Fuel Return', width: 100,   name: 'fuelReturn' },
                                { type: 'decimal', title: 'Housebuilding Loan', width: 100,  name: 'hbLoan' },
                                { type: 'decimal', title: 'Motorcycle Loan', width: 100,   name: 'mCylLoan' },
                                { type: 'decimal', title: 'Bicycle Loan', width: 100,   name: 'bCylLoan' },
                                { type: 'decimal', title: 'Computer Loan', width: 100,   name: 'computerLoan' },
                                { type: 'decimal', title: 'PF Loan', width: 100,  name: 'pfLoan' },
                                { type: 'decimal', title: 'WPF Loan', width: 100,  name: 'wpfLoan' },
                                { type: 'decimal', title: 'COS Loan', width: 100,   name: 'cosLoan' },
                                { type: 'decimal', title: 'Other Loan', width: 100,  name: 'otherLoan' },
                                { type: 'decimal', title: 'Advance', width: 100,   name: 'advance' },
                                { type: 'decimal', title: 'Others', width: 100,  name: 'others' },
                                { type: 'decimal', title: 'Pension Com', width: 100, name: 'pensionCom' },
                                { type: 'decimal', title: 'Utility Return', width: 100, name: 'utilityReturn' }
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

                        jss.deleteColumn(51, 2);
                    }
                    else {
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

        $.ajax({
            url: `/api/SalarySettings/CheckNetPayJS?monthId=${_monthId}`,
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
                    //showToast('Error', '', 'error');
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
                url: `/api/SalarySettings/UpdateFinalAdjustmentJuniorStaff`,
                contentType: 'application/json',
                dataType: 'json',
                type: 'POST',
                async: false,
                data: JSON.stringify({ finalAdjustmentJuniorStaff: updatedRow }),
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
            showToast(title = 'Error', message = 'No data found to update', toastrType = 'error');
        }
    });

});


function retrivedObject(rowData) {
    return {
        id: rowData[0],
        basicSalary: parseFloat(rowData[9]),
        personalSalary: parseFloat(rowData[10]),
        convenienceAllow: parseFloat(rowData[11]),
        arrearSalary: parseFloat(rowData[12]),
        workDays: parseFloat(rowData[13]),
        numberOfShift: parseFloat(rowData[14]),
        otherSalary: parseFloat(rowData[15]),
        specialBenefit: parseFloat(rowData[16]),
        lunchAllow: parseFloat(rowData[17]),
        tiffinAllow: parseFloat(rowData[18]),
        shiftAllow: parseFloat(rowData[19]),
        houseRentAllow: parseFloat(rowData[20]),
        familyMedicalAllow: parseFloat(rowData[21]),
        educationAllowance: parseFloat(rowData[22]),
        fieldAllow: parseFloat(rowData[23]),
        otSingle: parseFloat(rowData[24]),
        otDouble: parseFloat(rowData[25]),
        otAllow: parseFloat(rowData[26]),
        fuelAllow: parseFloat(rowData[27]),
        utilityAllow: parseFloat(rowData[28]),
        otherAllow: parseFloat(rowData[29]),
        revenueStamp: parseFloat(rowData[30]),
        providentFund: parseFloat(rowData[31]),
        welfareFund: parseFloat(rowData[32]),
        employeeClub: parseFloat(rowData[33]),
        employeeUnion: parseFloat(rowData[34]),
        dormitory: parseFloat(rowData[35]),
        hospitalDeduction: parseFloat(rowData[36]),
        specialDeduction: parseFloat(rowData[37]),
        fuelReturn: parseFloat(rowData[38]),
        hbLoan: parseFloat(rowData[39]),
        mCylLoan: parseFloat(rowData[40]),
        bCylLoan: parseFloat(rowData[41]),
        computerLoan: parseFloat(rowData[42]),
        pfLoan: parseFloat(rowData[43]),
        wpfLoan: parseFloat(rowData[44]),
        cosLoan: parseFloat(rowData[45]),
        otherLoan: parseFloat(rowData[46]),
        advance: parseFloat(rowData[47]),
        others: parseFloat(rowData[48]),
        pensionCom: parseFloat(rowData[49]),
        utilityReturn: parseFloat(rowData[50]),
        
    };
}


function getMonthId() {
    let month = $('#months').val();
    let year = $('#years').val();
    let monthId = (parseInt(year) * 100) + parseInt(month);
    $('#excel_export').attr('href', `/SalarySettings/FinalAdjustmentJuniorStaffExcelExport?monthId=${monthId}&month=${month}&year=${year}`);
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
