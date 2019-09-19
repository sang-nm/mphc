<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <div class="checkout-method">
      <h3 class="title-form">Phương thức thanh toán</h3>
      <div class="form-cart-wrap step-end clearfix">
        <xsl:if test="count(/CheckoutMethod/Payment)>0">
          <div class="clearfix mrb30">
            <ul class="nav">
              <xsl:apply-templates select="/CheckoutMethod/Payment"></xsl:apply-templates>
            </ul>
            <div class="agree">
              <input id="payAgree" type="checkbox" name="PaymentAgree"></input>
              <label for="payAgree">Tôi đã đọc và đồng ý điều khoản trên</label>
            </div>
          </div>
        </xsl:if>
      </div>
    </div>
    <div class="checkoutBtn">
      <div class="row">
        <div class="col-xs-12 col-md-4">
          <a class="checkout-back">
            <xsl:attribute name="href">
              <xsl:text>/cart</xsl:text>
            </xsl:attribute>
            <xsl:attribute name="target">
              <xsl:value-of select="Target"></xsl:value-of>
            </xsl:attribute>
            <i class="fa fa-long-arrow-left"></i>
            <xsl:text> Quay lại Giỏ hàng</xsl:text>
          </a>
        </div>
        <div class="col-xs-12 col-md-8 payment_wrap">
          <button type="submit" id="checkout" name="checkout" class="btn btn-payment">
            <xsl:attribute name="onclick">
              <xsl:text>AjaxCheckout.saveorder(true, '</xsl:text>
              <xsl:value-of select="/CheckoutMethod/NextPageUrl"></xsl:value-of>
              <xsl:text>');return false;</xsl:text>
            </xsl:attribute>
            <xsl:text>Đặt mua</xsl:text>
            <xsl:text> </xsl:text>
          </button>
        </div>
      </div>
    </div>
  </xsl:template>

  <xsl:template match="Payment">
    <li>
      <label>
        <span class="radpayment">
          <input type="radio" name="PaymentMethod">
            <xsl:attribute name="value">
              <xsl:value-of select="Id"></xsl:value-of>
            </xsl:attribute>
          </input>
        </span>
        <xsl:value-of select="Title"></xsl:value-of>
      </label>
      <xsl:if test="Description!=''">
        <div class="note">
          <xsl:value-of select="Description" disable-output-escaping="yes"></xsl:value-of>
        </div>
      </xsl:if>
    </li>
  </xsl:template>

</xsl:stylesheet>