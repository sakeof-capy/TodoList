$(document).ready(function() {
    console.log("Modal script loaded");

    function closeModal() {
        console.log("Closing modal");
        $('.modal-overlay').hide();
        $('.modal-content').hide();
    }

    $('.modal-overlay').click(function(e) {
        if ($(e.target).is('.modal-overlay')) {
            closeModal();
        }
    });

    $('.modal-close').click(function() {
        closeModal();
    });
});
