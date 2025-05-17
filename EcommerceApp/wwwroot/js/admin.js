/**
 * Admin Dashboard JavaScript
 */

document.addEventListener('DOMContentLoaded', function() {
    // Toast initialization
    const toastElList = document.querySelectorAll('.toast');
    toastElList.forEach(toastEl => {
        const toast = new bootstrap.Toast(toastEl, {
            autohide: true,
            delay: 3000
        });
    });

    // Function to show toast notifications
    window.showNotification = function(message, type = 'success') {
        const iconClass = type === 'success' ? 'bi-check-circle-fill' : 
                          type === 'error' ? 'bi-exclamation-triangle-fill' : 
                          type === 'warning' ? 'bi-exclamation-triangle-fill' : 'bi-info-circle-fill';
        
        const bgClass = type === 'success' ? 'bg-success' : 
                        type === 'error' ? 'bg-danger' : 
                        type === 'warning' ? 'bg-warning text-dark' : 'bg-info text-dark';
        
        const toast = `
            <div class="toast align-items-center text-white ${bgClass} border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        <i class="bi ${iconClass} me-2"></i>
                        ${message}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
        `;
        
        const toastContainer = document.querySelector('.toast-container');
        if (toastContainer) {
            toastContainer.innerHTML += toast;
            
            const toastElement = toastContainer.querySelector('.toast:last-child');
            const bsToast = new bootstrap.Toast(toastElement, { delay: 3000 });
            bsToast.show();
        }
    };

    // Table sorting functionality
    document.querySelectorAll('th[data-sort]').forEach(headerCell => {
        headerCell.addEventListener('click', () => {
            const tableElement = headerCell.closest('table');
            const headerIndex = Array.prototype.indexOf.call(headerCell.parentElement.children, headerCell);
            const currentIsAscending = headerCell.classList.contains('th-sort-asc');

            // Reset all headers
            tableElement.querySelectorAll('th').forEach(th => {
                th.classList.remove('th-sort-asc', 'th-sort-desc');
            });

            // Set the clicked header to the opposite sorting direction
            headerCell.classList.toggle('th-sort-asc', !currentIsAscending);
            headerCell.classList.toggle('th-sort-desc', currentIsAscending);

            // Get the tbody
            const tbody = tableElement.querySelector('tbody');
            // Get the rows
            const rows = Array.from(tbody.querySelectorAll('tr'));

            // Sort rows
            const sortedRows = rows.sort((a, b) => {
                const aColText = a.querySelector(`td:nth-child(${headerIndex + 1})`).textContent.trim();
                const bColText = b.querySelector(`td:nth-child(${headerIndex + 1})`).textContent.trim();

                return currentIsAscending 
                    ? bColText.localeCompare(aColText, undefined, {numeric: true})
                    : aColText.localeCompare(bColText, undefined, {numeric: true});
            });

            // Remove all existing rows
            while (tbody.firstChild) {
                tbody.removeChild(tbody.firstChild);
            }

            // Add sorted rows
            tbody.append(...sortedRows);
        });
    });

    // Form validation enhancement
    const forms = document.querySelectorAll('.needs-validation');
    
    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
                
                // Show validation errors
                const invalidInputs = form.querySelectorAll(':invalid');
                invalidInputs[0].focus();
            }
            
            form.classList.add('was-validated');
        }, false);
    });

    // Date range picker initialization
    const dateRangePickers = document.querySelectorAll('.date-range-picker');
    dateRangePickers.forEach(picker => {
        if (typeof daterangepicker !== 'undefined') {
            $(picker).daterangepicker({
                opens: 'left',
                autoUpdateInput: false,
                locale: {
                    cancelLabel: 'Clear',
                    applyLabel: 'Apply',
                    format: 'MM/DD/YYYY'
                }
            });
            
            $(picker).on('apply.daterangepicker', function(ev, picker) {
                $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
            });
            
            $(picker).on('cancel.daterangepicker', function(ev, picker) {
                $(this).val('');
            });
        }
    });

    // Image preview on file input change
    const imageInputs = document.querySelectorAll('.image-upload-input');
    imageInputs.forEach(input => {
        input.addEventListener('change', function() {
            const previewElement = document.querySelector(this.dataset.preview);
            if (previewElement && this.files && this.files[0]) {
                const reader = new FileReader();
                
                reader.onload = function(e) {
                    previewElement.src = e.target.result;
                }
                
                reader.readAsDataURL(this.files[0]);
            }
        });
    });
});
