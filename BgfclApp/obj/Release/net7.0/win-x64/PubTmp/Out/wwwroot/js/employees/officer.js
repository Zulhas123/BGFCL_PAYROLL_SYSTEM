let genders = [];
let maritals = [];
let religions = [];
let employeeTypes = [];
let grades = [];
let departments = [];
let designations = [];
let locations = [];
let activeStatus = [];
let _dataTable = undefined;
$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val());
    getEmployeeList(pageLength);
    $('#DepartmentId').select2({
        dropdownParent: $('#addEmployeeModal')
    });
    $('#DesignationId').select2({
        dropdownParent: $('#addEmployeeModal')
    });
    loadInitialData();
    $('#add_button').on('click', function () {
        $('#addEmployeeModal').modal('show');
        $('#EmployeeId').val('');
        $('#operation_type').val('create');
        $('#addEmployeeModalLabel').html('Add Employee');
    });
    getEmployeeList();
    {
        var navListItems = $('div.setup-panel div a'),
            allWells = $('.setup-content'),
            allNextBtn = $('.nextBtn');

        allWells.hide();

        navListItems.click(function (e) {
            e.preventDefault();
            var $target = $($(this).attr('href')),
                $item = $(this);

            if (!$item.hasClass('disabled')) {
                navListItems.removeClass('btn-success').addClass('btn-default');
                $item.addClass('btn-success');
                allWells.hide();
                $target.show();
                $target.find('input:eq(0)').focus();
            }
        });

        allNextBtn.click(function () {
            var curStep = $(this).closest(".setup-content"),
                curStepBtn = curStep.attr("id"),
                nextStepWizard = $('div.setup-panel div a[href="#' + curStepBtn + '"]').parent().next().children("a"),
                curInputs = curStep.find("input[type='text'],input[type='url']"),
                isValid = true;

            $(".form-group").removeClass("has-error");
            for (var i = 0; i < curInputs.length; i++) {
                if (!curInputs[i].validity.valid) {
                    isValid = false;
                    $(curInputs[i]).closest(".form-group").addClass("has-error");
                }
            }

            if (isValid) nextStepWizard.removeAttr('disabled').trigger('click');
        });

        $('div.setup-panel div a.btn-success').trigger('click');
    }

    
    $('#submit_button').on('click', function () {
        let employeeId = $('#EmployeeId').val();
        let jobCode = $('#JobCode').val();
        let employeeName = $('#EmployeeName').val();
        let fatherName = $('#FatherName').val();
        let motherName = $('#MotherName').val();
        let dateOfBirth = $('#DateOfBirth').val();
        let genderId = $('#GenderId').val();
        let maritalId = $('#MaritalId').val();
        let religionId = $('#ReligionId').val();

        let employeeTypeId = $('#EmployeeTypeId').val();
        let gradeId = $('#GradeId').val();
        let departmentId = $('#DepartmentId').val();
        let designationId = $('#DesignationId').val();
        let locationId = $('#LocationId').val();


        let joiningDate = $('#JoiningDate').val();
        /*let journalCode = $('#JournalCode').val();*/
        let tinNo = $('#TinNo').val();
        let nid = $('#Nid').val();
        let mobileNumber = $('#MobileNumber').val();
        let presentAddress = $('#PresentAddress').val();

        let permanentAddress = $('#PermanentAddress').val();
        let qualifications = $('#Qualifications').val();
        let identityMarks = $('#IdentityMarks').val();
        let remarks = $('#Remarks').val();
        let taxStatus = $('#TaxStatus').is(":checked");
        let activeStatus = $('#ActiveStatus').val();
        let empSl = $('#EmpSl').val();

        if (employeeId == '') {
            employeeId = 0;
        }

        let dataObj = {
            id: employeeId,
            jobCode: jobCode,
            employeeName: employeeName,
            fatherName: fatherName,
            motherName: motherName,

            dateOfBirth: dateOfBirth,
            genderId: genderId,
            maritalId: maritalId,
            religionId: religionId,
            employeeTypeId: employeeTypeId,

            schoolId: 1,
            guestPkId: 1,
            uuId: 1,
            guestUserId: 1,
            userId: 1,
            userTypeId: 1,
            roleId: 1,
            childCount: 2,

            gradeId: gradeId,
            departmentId: departmentId,
            designationId: designationId,
            locationId: locationId,
            joiningDate: joiningDate,

            journalCode: '',
            tinNo: tinNo,
            nid: nid,
            mobileNumber: mobileNumber,
            presentAddress: presentAddress,
            permanentAddress: permanentAddress,

            qualifications: qualifications,
            identityMarks: identityMarks,
            remarks: remarks,
            taxStatus: taxStatus,
            activeStatus: activeStatus,
            empSl: empSl
        };

        let operationType = $('#operation_type').val();

        if (operationType == 'create') {


            $.ajax({
                url: '/api/Employees/CreateEmployee',
                type: 'POST',
                async: false,
                data: dataObj
            }).always(function (responseObject) {
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    resetInputs();
                    $('#addEmployeeModal').modal('hide');
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    getEmployeeList();
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


        }
        else {

            $.ajax({
                url: '/api/Employees/UpdateEmployee',
                type: 'PUT',
                async: false,
                data: dataObj
            }).always(function (responseObject) {
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    resetInputs();
                    $('#addEmployeeModal').modal('hide');
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    getEmployeeList();
                    $('#operation_type').val('create');
                    $('#submit_button').html('create');
                }
                if (responseObject.statusCode == 400) {
                    for (let error in responseObject.errors) {
                        $(`#${error}`).empty();
                        $(`#${error}`).append(responseObject.errors[error]);
                    }
                    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                }
                if (responseObject.statusCode == 500 || responseObject.statusCode == 404) {
                    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                }
            });

        }


    });

    $('#cancel_button').on('click', () => {
        location.reload();
    });
    $('#cancel_button1').on('click', () => {
        location.reload();
    });
    $('#cancel_button2').on('click', () => {
        location.reload();
    });
    // Filter functionality
    $('#filter_input').on('keyup', function () {
        _dataTable.search(this.value).draw();
    });
    $('#page_length_select').on('change', function () {
        pageLength = parseInt($(this).val());
        getEmployeeList(pageLength);
    });

});


function resetInputs() {
    $('#JobCode').val('');
    $('#EmployeeName').val('');
    $('#FatherName').val('');
    $('#MotherName').val('');
    $('#DateOfBirth').val('');

    $('#JoiningDate').val('');
    $('#JournalCode').val('');
    $('#TinNo').val('');
    $('#MobileNumber').val('');
    $('#PresentAddress').val('');

    $('#PermanentAddress').val('');
    $('#Qualifications').val('');
    $('#IdentityMarks').val('');
    $('#Remarks').val('');
    $('#TaxStatus').prop('checked', false);

    loadInitialData();
}


function loadInitialData() {
    $.ajax({
        url: '/api/Others/GetGenders',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            genders = responseObject.data;
            $('#GenderId').empty();
            $('#GenderId').append('<option value="0">select one</option>');
            $.each(genders, function (key, item) {
                $('#GenderId').append(`<option value=${item.id}>${item.genderName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });

    $.ajax({
        url: '/api/Others/GetMaritals',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            maritals = responseObject.data;
            $('#MaritalId').empty();
            $('#MaritalId').append('<option value="0">select one</option>');
            $.each(maritals, function (key, item) {
                $('#MaritalId').append(`<option value=${item.id}>${item.maritalName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });

    $.ajax({
        url: '/api/Others/GetReligions',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            religions = responseObject.data;
            $('#ReligionId').empty();
            $('#ReligionId').append('<option value="0">select one</option>');
            $.each(religions, function (key, item) {
                $('#ReligionId').append(`<option value=${item.id}>${item.religionName}</option>`);
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
            $('#EmployeeTypeId').empty();
            $('#EmployeeTypeId').append('<option value="0">select one</option>');
            $.each(employeeTypes, function (key, item) {
                if (item.id == 1) {
                    $('#EmployeeTypeId').append(`<option value=${item.id}>${item.employeeTypeName}</option>`);
                }

            });
        },
        error: function (responseObject) {
        }
    });

    $.ajax({
        url: '/api/Grades/GetGradesByEmployeeType?employeeTypeId=1',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            grades = responseObject.data;
            $('#GradeId').empty();
            $('#GradeId').append('<option value="0">select one</option>');
            $.each(grades, function (key, item) {
                $('#GradeId').append(`<option value=${item.id}>${item.gradeName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });

    $.ajax({
        url: '/api/Departments/GetDepartments',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            departments = responseObject.data;
            $('#DepartmentId').empty();
            $('#DepartmentId').append('<option value="0">Select Department</option>');
            $.each(departments, function (key, item) {
                $('#DepartmentId').append(`<option value=${item.id}>${item.departmentName}</option>`);
            });
        },
        error: function (responseObject) {
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
                $('#department_filter').append(`<option value=${item.id}>${item.departmentName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });

    $.ajax({
        url: '/api/Designations/GetDesignationsByEmployeeType?employeeTypeId=1',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            designations = responseObject.data;
            $('#DesignationId').empty();
            $('#DesignationId').append('<option value="0">Select Designation</option>');
            $.each(designations, function (key, item) {
                $('#DesignationId').append(`<option value=${item.id}>${item.designationName}</option>`);
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
                $('#designation_filter').append(`<option value=${item.id}>${item.designationName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });
    $.ajax({
        url: '/api/Others/GetActiveStatus',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            activeStatus = responseObject.data;
            $('#ActiveStatus').empty();
            $.each(activeStatus, function (key, item) {
                $('#ActiveStatus').append(`<option value=${item.id}>${item.activeStatusName}</option>`);
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

}

function getEmployeeList(pageLength) {
    if ($.fn.DataTable.isDataTable('#employee_list_table')) {
        _dataTable.destroy();
    }

    // Get filter values properly
    let schoolId = $('#school_filter').val();
    let roleId = $('#role_filter').val();
    let departmentId = $('#department_filter').val();
    let designationId = $('#designation_filter').val();

    // Convert empty or null values to 0
    if (schoolId === "" || schoolId === null) schoolId = 0;
    if (roleId === "" || roleId === null) roleId = 0;
    if (departmentId === "" || departmentId === null) departmentId = 0;
    if (designationId === "" || designationId === null) designationId = 0;

    // Determine API URL based on filter selection
    let apiUrl = (schoolId != 0 || roleId != 0 || departmentId != 0 || designationId != 0)
        ? '/api/Employees/GetEmployeesWithFilter'
        : '/api/Employees/GetEmployees';
    //let apiUrl = '/api/Employees/GetEmployees';
    _dataTable = $('#employee_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url: apiUrl,
            type: 'GET',
            data: function (d) {
                d.employeeTypeId = 1;
                d.school_filter = schoolId;
                d.role_filter = roleId;
                d.department_filter = departmentId;
                d.designation_filter = designationId;
            },
            dataSrc: function (json) {
                if (typeof json === 'string') {
                    try {
                        json = JSON.parse(json);
                    } catch (e) {
                        alert('Invalid response format. Please refresh the page.');
                        return [];
                    }
                }

                return json?.data ?? [];
            },
            error: function (xhr, error, thrown) {
                console.error('AJAX error:', xhr.responseText);
                alert('Failed to load data. Server may be down ! you can Continue .');
            }
        },
        columns: [
            {
                data: '',
                className: 'text-center align-middle',
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                data: 'jobCode', orderable: false, className: 'text-center',
                render: function (data, type, row) {
                    return data || '';
                }
            },
            { data: 'employeeName', orderable: false,
            render: function (data, type, row) {
                return data || '';
            }
},
            {
                data: 'designationName', orderable: false,
                render: function (data, type, row) {
                    return data || '';
                } },
            {
                data: 'departmentName', orderable: false,
                render: function (data, type, row) {
                    return data || '';
                } },
            {
                data: 'activeStatus', orderable: false, className: 'text-center', render: function (data) {
                    switch (data) {
                        case 1: return '<span class="text-success">Active</span>';
                        case 2: return '<span class="text-warning">PRL</span>';
                        case 3: return '<span class="text-danger">In Active</span>';
                        default: return '<span class="text-secondary">Unknown</span>';
                    }
                }
            },
            {
                data: 'id',
                orderable: false,
                className: 'text-center align-middle',
                render: function (data) {
                    return `<button type="button" class="btn btn-primary btn-sm" onclick="onEditClicked(${data})">
                                <i class="fas fa-edit"></i>
                            </button> 
                            <button type="button" class="btn btn-danger btn-sm" onclick="onRemoveClicked(${data})">
                                <i class="fas fa-trash"></i>
                            </button>`;
                }
            }
        ],
        pageLength: 20,
        lengthMenu: [[10, 20, 50, 100, -1], [10, 20, 50, 100, 'All']],
        lengthChange: true,
        dom: '<"d-flex justify-content-between align-items-center mb-2"lfB>t<"d-flex justify-content-between align-items-center mt-2"ip>',
        buttons: ['copy', 'excel', 'pdf', 'print'],
        autoWidth: false,
        language: {
            lengthMenu: 'Data per page _MENU_  '
        },
        initComplete: function () {
            let api = this.api();

            // **Fix: Remove existing filters to prevent header height increase**
            $('#employee_list_table thead th').each(function () {
                $(this).children('.column-filter, br').remove();
            });

            // Add column filters dynamically (excluding the Action column)
            $('#employee_list_table thead th').each(function () {
                let title = $(this).text().trim();
                if (title && title !== 'Action' && title !== 'Sl.') {
                    $(this).append(`<input type="text" class="form-control form-control-sm column-filter" placeholder="Search" />`);
                }
            });


            // Apply column filtering
            api.columns().every(function () {
                let that = this;
                $('input.column-filter', this.header()).on('keyup change', function () {
                    if (that.search() !== this.value) {
                        that.search(this.value).draw();
                    }
                });
            });
        },
        autoWidth: false,
        scrollX: false
    });
}

// Event Listener for Filters
$('#school_filter, #role_filter, #department_filter, #designation_filter, #page_length_select').on('change', function () {
    getEmployeeList($('#page_length_select').val());
});



function onRemoveClicked(employeeId) {
    $('#remove_modal').modal('show');
    $('#EmployeeId').val(employeeId);
}

function onRemoveConfirmed() {
    $('#remove_spin').removeClass('d-none');
    let _employeetId = $('#EmployeeId').val();
    $('#remove_modal').modal('hide');
    $.ajax({
        url: '/api/Employees/DeleteEmployee?id=' + _employeetId,
        type: 'DELETE',
        async: false,
    }).always(function (responseObject) {
        if (responseObject.statusCode == 200) {
            getEmployeeList();
            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
        }
        if (responseObject.statusCode == 404 || responseObject.statusCode == 500) {
            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
        }
        $('#remove_spin').addClass('d-none');
    });
}
function onEditClicked(employeeId) {
    $('#addEmployeeModal').modal('show');
    // Set the operation type to 'edit' and update the button text
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#addEmployeeModalLabel').html('Edit Employee');
    let _employeeId = $('#EmployeeId').val();
    // Show a loading indicator or spinner if needed
    $('#remove_spin').removeClass('d-none');
    $.ajax({
        url: '/api/Employees/GetEmployeeById?id=' + employeeId,
        type: 'GET',
        async: false,
    }).always(function (responseObject) {
        if (responseObject.statusCode == 200) {
            $('#EmployeeId').val(responseObject.data.id);
            $('#JobCode').val(responseObject.data.jobCode);
            $('#EmployeeName').val(responseObject.data.employeeName);
            $('#FatherName').val(responseObject.data.fatherName);
            $('#MotherName').val(responseObject.data.motherName);
            $('#DateOfBirth').val(responseObject.data.dateOfBirth);

            $('#JoiningDate').val(responseObject.data.joiningDate);
            $('#JournalCode').val(responseObject.data.journalCode);
            $('#TinNo').val(responseObject.data.tinNo);
            $('#EmpSl').val(responseObject.data.empSl);
            $('#MobileNumber').val(responseObject.data.mobileNumber);
            $('#PresentAddress').val(responseObject.data.presentAddress);

            $('#PermanentAddress').val(responseObject.data.permanentAddress);
            $('#Qualifications').val(responseObject.data.qualifications);
            $('#IdentityMarks').val(responseObject.data.identityMarks);
            $('#Remarks').val(responseObject.data.remarks);
            $('#TaxStatus').prop('checked', responseObject.data.taxStatus);

            $('#GenderId').empty();
            $('#GenderId').append('<option value="0">select one</option>');
            $.each(genders, function (key, item) {
                if (item.id == responseObject.data.genderId) {
                    $('#GenderId').append(`<option value=${item.id} selected>${item.genderName}</option>`);
                }
                else {
                    $('#GenderId').append(`<option value=${item.id}>${item.genderName}</option>`);
                }

            });

            $('#MaritalId').empty();
            $('#MaritalId').append('<option value="0">select one</option>');
            $.each(maritals, function (key, item) {
                if (item.id == responseObject.data.maritalId) {
                    $('#MaritalId').append(`<option value=${item.id} selected>${item.maritalName}</option>`);
                }
                else {
                    $('#MaritalId').append(`<option value=${item.id}>${item.maritalName}</option>`);
                }

            });

            $('#ReligionId').empty();
            $('#ReligionId').append('<option value="0">select one</option>');
            $.each(religions, function (key, item) {
                if (item.id == responseObject.data.religionId) {
                    $('#ReligionId').append(`<option value=${item.id} selected>${item.religionName}</option>`);
                }
                else {
                    $('#ReligionId').append(`<option value=${item.id}>${item.religionName}</option>`);
                }

            });

            $('#EmployeeTypeId').empty();
            $('#EmployeeTypeId').append('<option value="0">select one</option>');
            $.each(employeeTypes, function (key, item) {
                if (item.id == responseObject.data.employeeTypeId) {
                    $('#EmployeeTypeId').append(`<option value=${item.id} selected>${item.employeeTypeName}</option>`);
                }
                else {
                    $('#EmployeeTypeId').append(`<option value=${item.id}>${item.employeeTypeName}</option>`);
                }

            });

            $('#GradeId').empty();
            $('#GradeId').append('<option value="0">select one</option>');
            $.each(grades, function (key, item) {
                if (item.id == responseObject.data.gradeId) {
                    $('#GradeId').append(`<option value=${item.id} selected>${item.gradeName}</option>`);
                }
                else {
                    $('#GradeId').append(`<option value=${item.id}>${item.gradeName}</option>`);
                }

            });

            $('#DepartmentId').empty();
            $('#DepartmentId').append('<option value="0">Select Department</option>');
            $.each(departments, function (key, item) {
                if (item.id == responseObject.data.departmentId) {
                    $('#DepartmentId').append(`<option value=${item.id} selected>${item.departmentName}</option>`);
                }
                else {
                    $('#DepartmentId').append(`<option value=${item.id}>${item.departmentName}</option>`);
                }

            });

            $('#DesignationId').empty();
            $('#DesignationId').append('<option value="0">Select Designation</option>');
            $.each(designations, function (key, item) {
                if (item.id == responseObject.data.designationId) {
                    $('#DesignationId').append(`<option value=${item.id} selected>${item.designationName}</option>`);
                }
                else {
                    $('#DesignationId').append(`<option value=${item.id}>${item.designationName}</option>`);
                }

            });

            $('#LocationId').empty();
            $('#LocationId').append('<option value="0">select one</option>');
            $.each(locations, function (key, item) {
                if (item.id == responseObject.data.locationId) {
                    $('#LocationId').append(`<option value=${item.id} selected>${item.locationName}</option>`);
                }
                else {
                    $('#LocationId').append(`<option value=${item.id}>${item.locationName}</option>`);
                }

            });

            $('#ActiveStatus').empty();
            $.each(activeStatus, function (key, item) {
                if (item.id == responseObject.data.activeStatus) {
                    $('#ActiveStatus').append(`<option value=${item.id} selected>${item.activeStatusName}</option>`);
                }
                else {
                    $('#ActiveStatus').append(`<option value=${item.id}>${item.activeStatusName}</option>`);
                }

            });


        }
        if (responseObject.statusCode == 404 || responseObject.statusCode == 500) {
            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
            $('#addEmployeeModal').modal('hide');
        }
        $('#remove_spin').addClass('d-none');
        $('#addEmployeeModal').modal('hide');
    });

}

