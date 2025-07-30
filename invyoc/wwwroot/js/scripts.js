function adjustHeight() {
    document.body.style.height = 'auto';
    let contentHeight = document.documentElement.scrollHeight;
    document.body.style.height = contentHeight + 'px';
}

window.addEventListener('load', adjustHeight);
window.addEventListener('resize', adjustHeight);

$("#spanCurrentYear").text(new Date().getFullYear());

function isNumberKey(evt, id) {
    try {
        var charCode = (evt.which) ? evt.which : event.keyCode;
        var input = document.getElementById(id);
        var value = input.value;

        // Allow only one dot (.)
        if (charCode == 46) {
            if (value.indexOf('.') === -1) {
                return true;
            } else {
                return false;
            }
        }

        // Allow digits (0-9)
        if (charCode >= 48 && charCode <= 57) {
            // If dot exists, check decimal length
            var dotIndex = value.indexOf('.');
            if (dotIndex > -1) {
                var decimals = value.substring(dotIndex + 1);
                // Prevent entering more than 2 decimal places
                if (input.selectionStart > dotIndex && decimals.length >= 2) {
                    return false;
                }
            }
            return true;
        }

        // Block other characters
        return false;

    } catch (e) {
        console.log(e);
        return false;
    }
}

function toINRCurrency(val) {
    return Number(val).toLocaleString('en-IN', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}


// Show Tour Information

const tourSteps = [
    {
        element: '#divInvoiceFrom',
        title: 'Your Company Information',
        content: 'This section lets to edit your company details that will appear on the invoice as "Invoice From".',
        placement: 'right'
    },
    {
        element: '#divInvoiceNum',
        title: 'Invoice Number',
        content: 'This section lets to enter your current Invoice Number.',
        placement: 'left'
    },
    {
        element: '#divBillTo',
        title: 'Billing Information',
        content: 'Here you can enter the customer details who will receive the invoice.',
        placement: 'bottom'
    },
    {
        element: '#divShipTo',
        title: 'Shipping Details',
        content: 'Here you can enter the shipping information (if different from billing).',
        placement: 'right'
    },
    {
        element: '#divOtherInvoiceInfo',
        title: 'Invoice Details',
        content: 'Here you can set the invoice dates, terms, and PO number.',
        placement: 'left'
    },
    {
        element: '#divInvoiceItems',
        title: 'Line Items',
        content: 'Add products or services here. Specify quantity, rate, and the amount will calculate automatically.',
        placement: 'top'
    },
    {
        element: '#divCurrencySection',
        title: 'Currency',
        content: 'Here you can select the desired currency for the invoice.',
        placement: 'left'
    },
    {
        element: '#divInvoiceTotals',
        title: 'Pricing Summary',
        content: 'This section allows to edit Discount & Tax percentage & then shows the subtotal, discounts, taxes, and the final total amount.',
        placement: 'left'
    },
    {
        element: '#divRemarksSection',
        title: 'Invoice Total',
        content: 'Add any special instructions or thank you messages for the customer.',
        placement: 'left'
    },
    {
        element: '#divDownloadInvoice',
        title: 'Finalize Invoice',
        content: 'Once you\'ve completed all sections, click here to download or send the invoice to your customer.',
        placement: 'left'
    }
];


document.addEventListener('DOMContentLoaded', function () {
    //if (!localStorage.getItem('invoiceTourCompleted')) {
    //    showTourConfirmation()
    //}
});

function showTourConfirmation() {
    const modal = document.createElement('div');
    modal.id = 'tourConfirmationModal';
    modal.style.position = 'fixed';
    modal.style.top = '0';
    modal.style.left = '0';
    modal.style.width = '100%';
    modal.style.height = '100%';
    modal.style.backgroundColor = 'rgba(0,0,0,0.5)';
    modal.style.display = 'flex';
    modal.style.justifyContent = 'center';
    modal.style.alignItems = 'center';
    modal.style.zIndex = '10000';

    const modalContent = document.createElement('div');
    modalContent.style.backgroundColor = 'white';
    modalContent.style.padding = '1rem';
    modalContent.style.borderRadius = '8px';
    modalContent.style.maxWidth = '350px';
    modalContent.style.textAlign = 'center';
    modalContent.style.boxShadow = '0 4px 8px rgba(0,0,0,0.1)';

    modalContent.innerHTML = `
        <h2 style="margin-top: 0; color: #4a4a4a;">Welcome to the Free Invoices</h2>
        <hr />
        <p style="margin-bottom: 2rem;">Would you like a quick tour to learn how to create your first invoice?</p>
        <div style="display: flex; justify-content: center; gap: 1rem;">
            <button id="startTourBtn" style="padding: 0.5rem; background-color: #4CAF50; color: white; border: none; border-radius: 4px; cursor: pointer;">
                Yes, Start Tour
            </button>
            <button id="skipTourBtn" style="padding: 0.5rem 1rem; background-color: #f44336; color: white; border: none; border-radius: 4px; cursor: pointer;">
                No, Skip Tour
            </button>
        </div>`;

    modal.appendChild(modalContent);
    document.body.appendChild(modal);

    document.getElementById('startTourBtn').addEventListener('click', function () {
        hideTourConfirmationModal(modal);
        loadBootstrapTourPlugin();
    });

    document.getElementById('skipTourBtn').addEventListener('click', function () {
        hideTourConfirmationModal(modal);
    });
}

function loadBootstrapTourPlugin() {
    // Check if Bootstrap Tour is already loaded
    const bootstrapTourCss = document.createElement('link');
    bootstrapTourCss.rel = 'stylesheet';
    bootstrapTourCss.href = 'https://cdnjs.cloudflare.com/ajax/libs/bootstrap-tour/0.12.0/css/bootstrap-tour-standalone.min.css';
    document.head.appendChild(bootstrapTourCss);

    const bootstrapTourJs = document.createElement('script');
    bootstrapTourJs.src = 'https://cdnjs.cloudflare.com/ajax/libs/bootstrap-tour/0.12.0/js/bootstrap-tour-standalone.min.js';
    bootstrapTourJs.onload = function () {
        initBootstrapTour();
    };
    document.head.appendChild(bootstrapTourJs);
}

function initBootstrapTour() {
    // Create overlay for dimming background
    const overlay = document.createElement('div');
    overlay.id = 'tour-overlay';
    overlay.style.position = 'fixed';
    overlay.style.top = '0';
    overlay.style.left = '0';
    overlay.style.width = '100%';
    overlay.style.height = '100%';
    overlay.style.zIndex = '1040';
    overlay.style.pointerEvents = 'none';
    document.body.appendChild(overlay);

    // Initialize tour
    const tour = new Tour({
        name: 'invoiceTour',
        backdrop: true,
        backdropPadding: 5,
        backdropContainer: 'body',
        container: 'body',
        keyboard: true,
        storage: false, // We'll handle storage ourselves
        debug: false,
        orphan: false,
        duration: false,
        delay: false,
        basePath: '',
        template: `
            <div class='popover tour'>
                <div class='arrow'></div>
                <h3 class='popover-title'></h3>
                <div class='popover-content'></div>
                <div class='popover-navigation'>
                    <div class='btn-group'>
                        <button class='btn btn-sm btn-default' data-role='prev'>« Back</button>
                        <button class='btn btn-sm btn-default' data-role='next'>Next »</button>
                        <button class='btn btn-sm btn-default' data-role='end'>End Tour</button>
                    </div>
                </div>
            </div>`,

        onShown: function (tour) {
            // Add highlight to current step element
            const step = tour.getStep(tour._current);
            if (step && step.element) {
                const element = document.querySelector(step.element);
                if (element) {
                    element.style.boxShadow = '0 0 0 2px rgba(81, 203, 238, 0.8)';
                    element.style.transition = 'box-shadow 0.3s ease';
                    element.style.position = 'relative';
                    element.style.zIndex = '1050';
                }
            }
        },

        onEnd: function () {
            // Clean up
            document.getElementById('tour-overlay').remove();
            localStorage.setItem('invoiceTourCompleted', 'true');
        },

        onHide: function (tour) {
            // Remove highlight from previous step element
            const step = tour.getStep(tour._current);
            if (step && step.element) {
                const element = document.querySelector(step.element);
                if (element) {
                    element.style.boxShadow = '';
                    element.style.zIndex = '';
                }
            }
        }
    });

    // Add steps
    tour.addSteps(tourSteps);

    // Start the tour
    tour.start();

    // Handle cleanup if tour is ended prematurely
    tour.on('cancel', function () {
        document.getElementById('tour-overlay').remove();
        localStorage.setItem('invoiceTourCompleted', 'true');
    });
}

function hideTourConfirmationModal(modal) {
    localStorage.setItem('invoiceTourCompleted', 'true');
    modal.remove();
}