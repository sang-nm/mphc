;
var wdH = 0,
    wdW = 0;

function winresize() {
    wdH = $(window).height();
    wdW = $(window).width();
}

;

function compareHeight(e) {
    var elementHeights = $(e).map(function() {
        return $(this).height();
    }).get();
    var maxHeight = Math.max.apply(null, elementHeights);
    $(e).height(maxHeight);
}

;
$(document).ready(function() {
    "use strict";
    winresize();

    $('a.scroll-down[href*=#]').on('click', function(event) {
        event.preventDefault();
        $('html,body').animate({ scrollTop: $(this.hash).offset().top }, 600);
    });


    if ($(window).width() >= 1200) {
        $('.dropdown').mouseover(function() {
            $(this).addClass("open");
        }).mouseout(function() {
            $(this).removeClass("open");
        });
        $('.mainMenu ul.nav > li > a').removeAttr("data-toggle");
    }

    //Search////////////////////////////////////////////////
    $(".dropdownSearchBtn").click(function() {
        $(".search").fadeToggle(200, function() {
            $("#searchinput").focus();
        });
        return false;
    });
    $(".searchclose").click(function() {
        $(".search").fadeToggle(200, function() {
            $(".dropdownSearchBtn").show();
        });
        return false;
    });

    $('.banner-slide').slick({
        arrows: false,
        dots: true,
        infinite: true,
        speed: 1000,
        slidesToShow: 1,
        adaptiveHeight: false,
        autoplay: true,
        autoplaySpeed: 3000,
    });

    var setHeightAb = $('.itemHeight');
    compareHeight(setHeightAb);

    var setHeightCt = $('.item-handBook').innerHeight();
    $('.item-handBook .contentWrap').css('min-height', setHeightCt);

    if (wdW >= 992) {
        var setHeightDl = $('.dealer-item').innerHeight();
        $('.dealer-item .dealerContentWrap').css('min-height', setHeightDl);
    }

    $('.block-productSalesList .owl-carousel, .news-special .owl-carousel').owlCarousel({
        items: 1,
        margin: 0,
        loop: false,
        dots: true,
        nav: false,
        lazyLoad: true,
        navRewind: true,
        autoplay: true,
        autoplayTimeout: 3000,
        autoplayHoverPause: true,
        slideBy: 1,
    });

    $('.icon-zoom').fancybox({
        'padding': 0,
        helpers: {
            overlay: {
                locked: false
            }
        }
    });

    $('.video-iframe, .fancybox-iframe').fancybox({
        'padding': 0,
        type: "iframe",
        helpers: {
            overlay: {
                locked: false
            }
        }
    });

    $('.popup-video').fancybox({
        'padding': 0,
        helpers: {
            overlay: {
                locked: false
            }
        }
    });

    $('.one-itemImg').slick({
        arrows: true,
        dots: false,
        infinite: true,
        speed: 300,
        slidesToShow: 1,
        adaptiveHeight: true
    });

});

;
$(window).resize(function() {
    "use strict";

    var setHeightAb = $('.itemHeight');
    compareHeight(setHeightAb);

});

;
$(window).load(function() {
    "use strict";

    var setHeightAb = $('.itemHeight');
    compareHeight(setHeightAb);

});

;
$(function() {
    "use strict";

    $('html').scrollUp({
        scrollName: 'scrollUp-text', // Element ID
        topDistance: '300', // Distance from top before showing element (px)
        topSpeed: 300, // Speed back to top (ms)
        animation: 'fade', // Fade, slide, none
        animationInSpeed: 200, // Animation in speed (ms)
        animationOutSpeed: 200, // Animation out speed (ms)
        scrollText: '<i class="fa fa-angle-double-up" aria-hidden="true"></i>', // Text for element
        activeOverlay: false // Set CSS color to display scrollUp active point, e.g '#00FFFF'
    });

    //Menu Mobile
    $('#header_icon').click(function(e) {
        e.preventDefault();
        $('body').toggleClass('with-sidebar');
    });

    $('#site-cache, #btnMenuMain').click(function(e) {
        $('body').removeClass('with-sidebar');
    });

    initPointSlider();
    function initPointSlider() {
        if ($('#point-slider').length > 0) {
            var maxValue = $('#hdfMaxPoints').val();

            var options =
            {
                range: false,
                step: 10,
                min: 0,
                max:  maxValue,
                values: 0,
                slide: function (event, ui) {
                    $("#currentPoint").text(ui.value);
                    $("#hdfCurrentPoint").val(ui.value);
                },
                change: function (event, ui) {
                    var data = { 'method': 'ChangePoint', 'point': $("#hdfCurrentPoint").val() };
                    $.ajax({
                        cache: false,
                        url: siteRoot + "/Product/Services/CheckoutService.aspx",
                        data: data,
                        type: 'post',
                        success: function (result) {
                            if(result.success)
                                {
                                    $('.point-discount').html(result.pointDiscount);
                                    $('.order-total').html(result.total);
                                }
                            else
                                alert(result.message);
                        }
                    });
                }
            };

            $("#point-slider").slider(options);
        }
    }

    $(".loginFb,.loginfacebook,.registerfacebook").on('click', function () {
        var url = $(this).data('url');
        window.open(url, "popupWindow", "width=640,height=480,scrollbars=yes");
        return false;
    })

});

;
$(window).scroll(function() {

});