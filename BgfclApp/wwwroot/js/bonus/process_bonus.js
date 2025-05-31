let _dataTable = undefined;
$(document).ready(function () {
    loadInitialData();
    getBonusprocessList();
    $('#festival_checkbox').on('change', function () {
        if ($(this).is(':checked')) {
            $('#festival_basic').attr('required', true);
            $('#incentive_checkbox, #honorarium_checkbox, #scholarship_checkbox').prop('disabled', true);
            $('#muslim_checkbox, #hindu_checkbox, #buddist_checkbox, #christian_checkbox').prop('disabled', false);
        } else {
            $('#festival_basic').removeAttr('required');
            $('#incentive_checkbox, #honorarium_checkbox, #scholarship_checkbox').prop('disabled', false);
            $('#muslim_checkbox, #hindu_checkbox, #buddist_checkbox, #christian_checkbox').prop('disabled', true);
        }
    });
    $('#bonus_form').on('submit', function (e) {
        if ($('#festival_checkbox').is(':checked')) {
            const isAnyReligionChecked = $('#muslim_checkbox').is(':checked') ||
                $('#hindu_checkbox').is(':checked') ||
                $('#buddist_checkbox').is(':checked') ||
                $('#christian_checkbox').is(':checked');

            if (!isAnyReligionChecked) {
                e.preventDefault(); // Stop form submission
                alert('Please select at least one religion when Festival Bonus is checked.');
                return false;
            }
        }
    });


    $('#incentive_checkbox').on('change', function () {
        if ($(this).is(':checked')) {
            $('#festival_checkbox, #honorarium_checkbox, #scholarship_checkbox').prop('disabled', true);
            $('#muslim_checkbox, #hindu_checkbox, #buddist_checkbox, #christian_checkbox').prop('disabled', false);
        } else {
            $('#festival_checkbox, #honorarium_checkbox, #scholarship_checkbox').prop('disabled', false);
            $('#muslim_checkbox, #hindu_checkbox, #buddist_checkbox, #christian_checkbox').prop('disabled', true);
        }
    });
    $('#honorarium_checkbox').on('change', function () {
        if ($(this).is(':checked')) {
            $('#festival_checkbox, #incentive_checkbox, #scholarship_checkbox').prop('disabled', true);
            $('#muslim_checkbox, #hindu_checkbox, #buddist_checkbox, #christian_checkbox').prop('disabled', false);
        } else {
            $('#festival_checkbox, #incentive_checkbox, #scholarship_checkbox').prop('disabled', false);
            $('#muslim_checkbox, #hindu_checkbox, #buddist_checkbox, #christian_checkbox').prop('disabled', true);
        }
    });
    $('#scholarship_checkbox').on('change', function () {
        if ($(this).is(':checked')) {
            $('#festival_checkbox, #incentive_checkbox, #honorarium_checkbox').prop('disabled', true);
            $('#muslim_checkbox, #hindu_checkbox, #buddist_checkbox, #christian_checkbox').prop('disabled', false);
        } else {
            
            $('#festival_checkbox, #incentive_checkbox, #honorarium_checkbox').prop('disabled', false);
            $('#muslim_checkbox, #hindu_checkbox, #buddist_checkbox, #christian_checkbox').prop('disabled', true);
        }
    });

    $('#muslim_checkbox, #hindu_checkbox, #buddist_checkbox, #christian_checkbox').prop('disabled', true);
    $('#months').on('change', function () {
        let monthId = getMonthId();
        $('#bonus_form').css('display', 'none');
        $.ajax({
            url: '/api/Bonus/GetBonusByMonthId?monthId=' + monthId,
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (responseObject) {
                let bonus = JSON.parse(JSON.stringify(responseObject.data));
                $('#BonusId').empty();
                $('#BonusId').append(`<option value='0'>Select Bonus</option>`);
                $.each(bonus, function (key, item) {
                    $('#BonusId').append(`<option value=${item.Id}>${item.bonusTitle}</option>`);

                });
            },
            error: function (responseObject) {
            }
        });

    });

    $('#years').on('change', function () {
        let monthId = getMonthId();
        $('#bonus_form').css('display', 'none');
        $.ajax({
            url: '/api/Bonus/GetBonusByMonthId?monthId=' + monthId,
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (responseObject) {
                let bonus = JSON.parse(JSON.stringify(responseObject.data));
                $('#BonusId').empty();
                $('#BonusId').append(`<option value='0'>Select Bonus</option>`);
                $.each(bonus, function (key, item) {
                    $('#BonusId').append(`<option value=${item.id}>${item.bonusTitle}</option>`);

                });
            },
            error: function (responseObject) {
            }
        });
    });

    $('#BonusId').on('change', () => {
        let bonusId = $('#BonusId').val();
        if (parseInt(bonusId) > 0) {
            $('#bonus_form').css('display', 'block');
        }
    });

    $('#process_button').on('click', () => {

        let isValid = true; 
        $('.form-check-input').each(function () {
            const isChecked = $(this).is(':checked');
            const associatedInput = $(this).closest('.form-group').find('.inputfield');

            if (isChecked && associatedInput.length > 0) {
                const inputValue = associatedInput.val().trim();

                if (inputValue === "") {
                    isValid = false;
                    alert(`The field for "${$(this).next('label').text().trim()}" is required.`); 
                    associatedInput.focus(); 
                    return false; 
                }
            }
        });

        if (!isValid) {
            return; 
        }

        console.log("Click: ")
        let bonusId = $('#BonusId').val();
        let employeeType = $('#EmployeeTypeId').val();
        var file = $('#excelFileInput').prop('files')[0];
        console.log(file);
        let dataObj = {
            bonusId: bonusId,
            bonusName: '',
            EmployeeTypeId: employeeType,
            hindu: false,
            muslim: false,
            buddist: false,
            christian: false,
            basic: 0,
            dataType:'n'
        };

        let festivalBonus = $('#festival_checkbox').is(":checked");
        let incentiveBonus = $('#incentive_checkbox').is(":checked");
        let honorariumBonus = $('#honorarium_checkbox').is(":checked");
        let scholarshipBonus = $('#scholarship_checkbox').is(":checked");

        // Function to read Excel file (this can be replaced with your logic to handle the file)


        if (festivalBonus) {
            dataObj.bonusName = 'festival';

            let basic = $('#festival_basic').val();
            basic = basic == '' ? 0 : basic;
            dataObj.basic = basic;

            let muslim = $('#muslim_checkbox').is(":checked");
            let hindu = $('#hindu_checkbox').is(":checked");
            let buddist = $('#buddist_checkbox').is(":checked");
            let christian = $('#christian_checkbox').is(":checked");
            if (muslim) {
                dataObj.muslim = true;
            }
            if (hindu) {
                dataObj.hindu = true;
            }
            if (buddist) {
                dataObj.buddist = true;
            }
            if (christian) {
                dataObj.christian = true;
            }

            $.ajax({
                url: '/api/Bonus/ProcessBonus',
                type: 'POST',
                async: false,
                data: dataObj,
                success: function (responseObject) {
                    if (responseObject.statusCode == 201) {
                        showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    }
                },
                error: function (responseObject) {
                }
            });
        }

        // Check if any bonus checkbox is checked, and if the corresponding basic value is empty or 0
        if (incentiveBonus) {

            dataObj.bonusName = 'incentive';
            let basic = $('#incentive_basic').val();
            basic = basic == '' ? 0 : basic;
            dataObj.basic = basic;
            

            if (file == undefined) {
                dataObj.dataType = 'n';
                $.ajax({
                    url: '/api/Bonus/ProcessBonus',
                    type: 'POST',
                    async: false,
                    data: dataObj,
                    success: function (responseObject) {
                        if (responseObject.statusCode == 201) {
                            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                        }
                        if (responseObject.statusCode == 500) {
                            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                        }
                    },
                    error: function (responseObject) {
                    }
                });
            }
            else {
                dataObj.dataType = 'f';
                let reader = new FileReader();
                reader.readAsArrayBuffer(file);
                reader.onload = function (e) {
                    let data = new Uint8Array(e.target.result);
                    let workbook = XLSX.read(data, { type: 'array' });
                    let sheetName = workbook.SheetNames[0];
                    let worksheet = workbook.Sheets[sheetName];
                    let jsonData = XLSX.utils.sheet_to_json(worksheet, { header: 1 });

                    let _dataString = '';
                    for (var i = 1; i < jsonData.length; i++) {
                        _dataString += jsonData[i][0] + '_' + jsonData[i][1] + '#';
                    }
                    dataObj.dataString = _dataString;
                    $.ajax({
                        url: '/api/Bonus/ProcessBonus',
                        type: 'POST',
                        async: false,
                        data: dataObj,
                        success: function (responseObject) {
                            if (responseObject.statusCode == 201) {
                                showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                            }
                            if (responseObject.statusCode == 500) {
                                showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                            }
                        },
                        error: function (responseObject) {
                        }
                    });

                };
            }


        }

        if (honorariumBonus) {

            dataObj.bonusName = 'honorarium';
            let basic = $('#honorarium_basic').val();
            basic = basic == '' ? 0 : basic;
            dataObj.basic = basic;


            if (file == undefined) {
                dataObj.dataType = 'n';
                $.ajax({
                    url: '/api/Bonus/ProcessBonus',
                    type: 'POST',
                    async: false,
                    data: dataObj,
                    success: function (responseObject) {
                        if (responseObject.statusCode == 201) {
                            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                        }
                        if (responseObject.statusCode == 500) {
                            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                        }
                    },
                    error: function (responseObject) {
                    }
                });
            }
            else {
                dataObj.dataType = 'f';
                let reader = new FileReader();
                reader.readAsArrayBuffer(file);
                reader.onload = function (e) {
                    let data = new Uint8Array(e.target.result);
                    let workbook = XLSX.read(data, { type: 'array' });
                    let sheetName = workbook.SheetNames[0];
                    let worksheet = workbook.Sheets[sheetName];
                    let jsonData = XLSX.utils.sheet_to_json(worksheet, { header: 1 });

                    let _dataString = '';
                    for (var i = 1; i < jsonData.length; i++) {
                        _dataString += jsonData[i][0] + '_' + jsonData[i][1] + '#';
                    }
                    dataObj.dataString = _dataString;
                    $.ajax({
                        url: '/api/Bonus/ProcessBonus',
                        type: 'POST',
                        async: false,
                        data: dataObj,
                        success: function (responseObject) {
                            if (responseObject.statusCode == 201) {
                                showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                            }
                            if (responseObject.statusCode == 500) {
                                showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                            }
                        },
                        error: function (responseObject) {
                        }
                    });

                };
            }


        }
        if (scholarshipBonus) {

            dataObj.bonusName = 'scholarship';
            let basic = $('#scholarship_basic').val();
            basic = basic == '' ? 0 : basic;
            dataObj.basic = basic;


            if (file == undefined) {
                dataObj.dataType = 'n';
                $.ajax({
                    url: '/api/Bonus/ProcessBonus',
                    type: 'POST',
                    async: false,
                    data: dataObj,
                    success: function (responseObject) {
                        if (responseObject.statusCode == 201) {
                            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                        }
                        if (responseObject.statusCode == 500) {
                            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                        }
                    },
                    error: function (responseObject) {
                    }
                });
            }
            else {
                dataObj.dataType = 'f';
                let reader = new FileReader();
                reader.readAsArrayBuffer(file);
                reader.onload = function (e) {
                    let data = new Uint8Array(e.target.result);
                    let workbook = XLSX.read(data, { type: 'array' });
                    let sheetName = workbook.SheetNames[0];
                    let worksheet = workbook.Sheets[sheetName];
                    let jsonData = XLSX.utils.sheet_to_json(worksheet, { header: 1 });

                    let _dataString = '';
                    for (var i = 1; i < jsonData.length; i++) {
                        _dataString += jsonData[i][0] + '_' + jsonData[i][1] + '#';
                    }
                    dataObj.dataString = _dataString;
                    $.ajax({
                        url: '/api/Bonus/ProcessBonus',
                        type: 'POST',
                        async: false,
                        data: dataObj,
                        success: function (responseObject) {
                            if (responseObject.statusCode == 201) {
                                showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                            }
                            if (responseObject.statusCode == 500) {
                                showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                            }
                        },
                        error: function (responseObject) {
                        }
                    });

                };
            }


        }

       
    });

});

function getBonusprocessList() {
    let schoolId = parseInt($('#school_filter').val()) || 0;
    let roleId = parseInt($('#role_filter').val()) || 0;

    // Keep department and designation as strings
    let department = $('#department_filter').val()?.trim();
    let designation = $('#designation_filter').val()?.trim();

    let employeeTypeId = parseInt($('#employee_type_filter').val()) || 1;

    console.log("employeeTypeId:", employeeTypeId);

    // Ensure department and designation are treated properly
    if (department === "0" || department === null || department === "") department = "";
    if (designation === "0" || designation === null || designation === "") designation = "";

    // Determine API URL based on filter selection
    let apiUrl = (schoolId > 0 || roleId > 0 || department !== "" || designation !== "")
        ? '/api/Bonus/GetBonusProcessDataWithFilter'
        : '/api/Bonus/GetAllBonusProcessData';

    console.log("API URL:", apiUrl);

    // Destroy existing DataTable instance if it exists
    if ($.fn.DataTable.isDataTable("#process_bonus_list_table")) {
        $('#process_bonus_list_table').DataTable().destroy();
    }
    $('#process_bonus_list_table tbody').empty();
    // Initialize DataTable
    _dataTable = $('#process_bonus_list_table').DataTable({
        order: [[1, 'desc'], [2, 'desc']],
        pageLength: 20, // Set default page length
        lengthMenu: [[10, 20, 50, 100], [10, 20, 50, 100]], // Page length options
        ajax: {
            url: apiUrl,
            type: 'GET',
            data: function (d) {
                d.employeeTypeId = employeeTypeId;
                d.school_filter = schoolId;
                d.role_filter = roleId;
                d.department_filter = department;
                d.designation_filter = designation;
            },
            dataSrc: function (json) {
                if (!json.data || json.data.length === 0) return [];

                // Filter out rows where all values are null or empty
                return json.data.filter(row =>
                    Object.values(row).some(val => val !== null && val !== '')
                );
            }

        },

        columns: [
            {
                data: null, className: 'text-center', render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                data: 'jobCode', className: 'text-center', orderable: false, render: function (data, type, row) {
                    return data || '';
                }
            },
            {
                data: 'employeeName', className: 'text-left', orderable: false, render: function (data, type, row) {
                    return data || '';
                }
            },
            {
                data: 'designationName', className: 'text-left', orderable: false, render: function (data, type, row) {
                    return data || '';
                }
            },
            {
                data: 'departmentName', className: 'text-left', orderable: false, orderable: false, render: function (data, type, row) {
                    return data || '';
                }
            },
            { data: 'festivalBonus', className: 'text-right', orderable: false, render: data => data ? parseFloat(data).toLocaleString() : '-' },
            { data: 'incentiveBonus', className: 'text-right', orderable: false, render: data => data ? parseFloat(data).toLocaleString() : '-' },
            { data: 'honorariumBonus', className: 'text-right', orderable: false, render: data => data ? parseFloat(data).toLocaleString() : '-' },
            { data: 'scholarshipBonus', className: 'text-right', orderable: false, render: data => data ? parseFloat(data).toLocaleString() : '-' },
            { data: 'revStamp', className: 'text-right', orderable: false, render: data => data ? parseFloat(data).toLocaleString() : '-' },
            { data: 'deduction', className: 'text-right', orderable: false, render: data => data ? parseFloat(data).toLocaleString() : '-' }
        ],

        pageLength: 20,
        lengthMenu: [[10, 20, 50, 100, -1], [10, 20, 50, 100, 'All']],
        lengthChange: true,
        //dom: '<"d-flex justify-content-between align-items-center"Bf>tip',
        dom: '<"d-flex justify-content-between align-items-center mb-2"lfB>t<"d-flex justify-content-between align-items-center mt-2"ip>',
        buttons: ['copy', 'excel', 'pdf', 'print'],        
        language: {
            lengthMenu: 'Data per page <select class="form-control form-select form-select-sm">' +
                '<option value="10">10</option>' +
                '<option value="20" selected>20</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '<option value="-1">All</option>' +
                '</select>',
            emptyTable: 'No salary process data available',
            loadingRecords: 'Loading...',
            zeroRecords: 'No matching records found'
        },
        responsive: true,
        autoWidth: false
    });
}




// Call getBonusprocessList when filters are changed
$('#school_filter, #role_filter, #department_filter, #designation_filter, #employee_type_filter').on('change', function () {
    getBonusprocessList(); // Ensure this is the correct function name
});


function getMonthId() {
    let month = $('#months').val();
    let year = $('#years').val();

    return (parseInt(year) * 100) + parseInt(month);
}

function loadInitialData() {
    $.ajax({
        url: '/api/Employees/GetEmployeeTypes',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            employeeTypes = responseObject.data;
            $('#EmployeeTypeId').empty();
            $.each(employeeTypes, function (key, item) {
                $('#EmployeeTypeId').append(`<option value=${item.id}>${item.employeeTypeName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });

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
               // let selected = item == currentYear ? 'selected' : '';
                $('#years, #f_years').append(`<option value=${item}>${item}</option>`);
            });
        },
        error: function () {
            alert('Failed to load years.');
        }
    });

    $.ajax({
        url: '/api/Departments/GetDepartments',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            departments = responseObject.data;
            $('#department_filter').empty();
            $('#department_filter').append('<option value="0">All Department</option>');
            $.each(departments, function (key, item) {
                $('#department_filter').append(`<option value=${item.departmentName}>${item.departmentName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });
    $.ajax({
        url: '/api/Designations/GetDesignations',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            designations = responseObject.data;
            $('#designation_filter').empty();
            $('#designation_filter').append('<option value="0">All Designation</option>');
            $.each(designations, function (key, item) {
                $('#designation_filter').append(`<option value=${item.designationName}>${item.designationName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });
    $.ajax({
        url: '/api/Roles/GetRole',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            roles = responseObject.data;
            $('#role_filter').empty();
            $('#role_filter').append('<option value="0">All Roles</option>');
            $.each(roles, function (key, item) {
                $('#role_filter').append(`<option value=${item.id}>${item.title}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });
    $.ajax({
        url: '/api/Schools/GetSchools',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            roles = responseObject.data;
            $('#school_filter').empty();
            $('#school_filter').append('<option value="0">All School</option>');
            $.each(roles, function (key, item) {
                $('#school_filter').append(`<option value=${item.id}>${item.title}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });
    $.ajax({
        url: '/api/Employees/GetEmployeeTypes',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            employeeTypes = responseObject.data;
            $('#employee_type_filter').empty();
            $('#employee_type_filter').append('<option value="0">All Types</option>');
            $.each(employeeTypes, function (key, item) {
                $('#employee_type_filter').append(`<option value=${item.id}>${item.employeeTypeName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });
}


