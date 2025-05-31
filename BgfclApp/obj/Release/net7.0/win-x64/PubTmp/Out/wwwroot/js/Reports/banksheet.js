$(document).ready(function () {
    loadInitialData();

    $('#isExcel').on('change', function () {
        if ($(this).is(':checked')) {
            $('#generateButton')
                .text('Export To Excel')                     // Change text
                .css('background-color', 'yellow') // Set background color to yellow
                .css('color', 'black');            // Optional: change text color for better contrast
        } else {
            $('#generateButton')
                .text('Report To PDF')                    // Change text back
                .css('background-color', '')       // Reset background color to default
                .css('color', '');                 // Reset text color
        }
    });
    setTimeout(function () {
        var alertBox = document.getElementById("processAlert");
        if (alertBox) {
            alertBox.classList.remove("show");
            alertBox.classList.add("fade");
            setTimeout(() => {
                alertBox.style.display = 'none';
            }, 500); // Allow Bootstrap's fade animation
        }
    }, 5000);
});
function getMonthId() {
    let month = $('#months').val();
    let year = $('#years').val();

    return (parseInt(year) * 100) + parseInt(month);
}

function loadInitialData() {
    // Load Employee Types
    $.ajax({
        url: '/api/Employees/GetEmployeeTypes',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let employeeTypes = responseObject.data;
            const $employeeType = $('#EmployeeTypeId');
            const $errorField = $('#EmployeeTypeIdError');

            $employeeType.empty();
            $employeeType.append('<option value="">Select One</option>');

            $.each(employeeTypes, function (key, item) {
                // Append only item with id 1 or 2
                if (item.id == 1 || item.id == 2 || item.id == 3) {
                    $employeeType.append(`<option value="${item.id}">${item.employeeTypeName}</option>`);
                }
            });

            // Reset error field
            $errorField.text('');
        },
        error: function (xhr, status, error) {
            console.error("Error fetching employee types:", error);
        }
    });

    // Load Months
    $.ajax({
        url: '/api/Others/GetMonths',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let months = responseObject.data;
            let currentMonth = new Date().getMonth() + 1;
            const $months = $('#months');

            $months.empty();
            $.each(months, function (key, item) {
                $months.append(`<option value="${item.monthId}" ${currentMonth === parseInt(item.monthId) ? 'selected' : ''}>${item.monthName}</option>`);
            });
        },
        error: function (responseObject) {
            console.error("Error loading months");
        }
    });

    // Load Years
    $.ajax({
        url: '/api/Others/GetYears',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let years = responseObject.data;
            let currentYear = new Date().getFullYear();
            const $years = $('#years');

            $years.empty();
            $.each(years, function (key, item) {
                $years.append(`<option value="${item}" ${item === currentYear ? 'selected' : ''}>${item}</option>`);
            });
        },
        error: function (responseObject) {
            console.error("Error loading years");
        }
    });
    $('#salaryForm').on('submit', function (e) {
        const employeeTypeValue = $('#EmployeeTypeId').val();
        const $errorField = $('#EmployeeTypeIdError');

        $errorField.text('');

        if (!employeeTypeValue) {
            e.preventDefault();
            $errorField.text('Please select an employee type.');
        }
    });

}
$('#EmployeeTypeId').on('change', function () {
    const employeeTypeValue = $(this).val();
    const $errorField = $('#EmployeeTypeIdError');

    if (employeeTypeValue) {
        $errorField.text('');
    }
});