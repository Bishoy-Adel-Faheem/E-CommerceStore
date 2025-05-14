// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Function to update the cart badge with current count
function updateCartBadge() {
    $.ajax({
        url: '/Cart/GetCartCount',
        type: 'GET',
        success: function (data) {
            const count = data.count || 0;
            $('#cart-badge').text(count);
            
            // Hide badge if count is 0
            if (count === 0) {
                $('#cart-badge').addClass('visually-hidden');
            } else {
                $('#cart-badge').removeClass('visually-hidden');
            }
        }
    });
}

// Update cart badge on page load
$(document).ready(function () {
    updateCartBadge();
    
    // Handle add to cart forms with AJAX
    $(document).on('submit', '.add-to-cart-form', function (e) {
        e.preventDefault();
        const form = $(this);
        const url = form.attr('action');
        const data = form.serialize();
        
        $.ajax({
            type: 'POST',
            url: url,
            data: data,
            success: function (response) {
                if (response.success) {
                    // Update the cart badge
                    $('#cart-badge').text(response.count);
                    $('#cart-badge').removeClass('visually-hidden');
                      // Show a success message
                    const toast = $('<div class="toast align-items-center text-white bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">' +
                        '<div class="d-flex">' +
                        '<div class="toast-body">Item added to cart!</div>' +
                        '<button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>' +
                        '</div>' +
                        '</div>');
                    
                    // Append to toast container and show
                    $('.toast-container').append(toast);
                    const bsToast = new bootstrap.Toast(toast[0], {
                        autohide: true,
                        delay: 3000
                    });
                    bsToast.show();
                    
                    // Remove from DOM after hiding
                    toast.on('hidden.bs.toast', function () {
                        toast.remove();
                    });
                }
            }
        });
    });
});
