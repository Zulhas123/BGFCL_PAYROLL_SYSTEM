$(document).ready(function () {
    $(function () {
        var current = window.location.pathname + window.location.search;
        $('#sidebar-menu ul li.submenu a').each(function () {
            var $this = $(this);
            // if the current path is equal to this link, add class menu-active
            if ($this.attr('href') === current) {
                $this.addClass('active');
                $this.parent().parent().slideToggle("slow");
            }
        })
    });
});

function showToast(title, message, toastrType) {
    toastr[toastrType](message, title)

    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
}

// Global loader object
var AppLoader = {
    // Initialize the loader
    init: function () {
        // Create loader HTML if not exists
        if (!$('#globalLoader').length) {
            $('body').append(`
                <div id="globalLoader" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%;
                     background-color: rgba(255,255,255,0.6); z-index: 9999;">
                    <div class="d-flex justify-content-center align-items-center" style="height: 100%;">
                        <img src="/image/loader.gif" alt="Loading..." style="width: 80px; height: 80px;" />
                    </div>
                </div>
            `);
        }
    },

    // Show the loader
    show: function () {
        this.init();
        $('#globalLoader').show();
    },

    // Hide the loader
    hide: function () {
        $('#globalLoader').hide();
    },

    // Wrap an async function with loader
    wrap: async function (asyncFunction) {
        try {
            this.show();
            return await asyncFunction();
        } finally {
            this.hide();
        }
    }
};

// Show loader immediately when script loads (before DOM ready)
AppLoader.init();
AppLoader.show();

// Hide loader when DOM is fully loaded
$(window).on('load', function () {
    AppLoader.hide();
});

// Initialize other things when DOM is ready
$(document).ready(function () {

    // Global AJAX loader handling
    $(document).ajaxStart(function () {
        AppLoader.show();
    });

    $(document).ajaxStop(function () {
        AppLoader.hide();
    });

    $(document).ajaxError(function () {
        AppLoader.hide();
    });
});
