<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <div class="checkout-address form-join-wrap form-horizontal">
      <h3 class="title-form">
        <xsl:text>Thông tin khách hàng</xsl:text>
      </h3>
      <div class="form-group">
        <div class="row">
          <label class="col-md-3 col-xs-12">
            <xsl:value-of select="/CheckoutAddress/FirstNameText"></xsl:value-of>
            <span>(*)</span>
          </label>
          <div class="col-md-9 col-xs-12">
            <input type="text" name="Address_FirstName" class="form-control">
              <xsl:attribute name="value">
                <xsl:value-of select="/CheckoutAddress/FirstName"></xsl:value-of>
              </xsl:attribute>
            </input>
          </div>
        </div>
      </div>
      <div class="form-group">
        <div class="row">
          <label class="col-md-3 col-xs-12">
            <xsl:value-of select="/CheckoutAddress/EmailText"></xsl:value-of>
          </label>
          <div class="col-md-9 col-xs-12">
            <input type="text" name="Address_Email" class="form-control">
              <xsl:attribute name="value">
                <xsl:value-of select="/CheckoutAddress/Email"></xsl:value-of>
              </xsl:attribute>
            </input>
          </div>
        </div>
      </div>
      <div class="form-group">
        <div class="row">
          <label class="col-md-3 col-xs-12">
            <xsl:value-of select="/CheckoutAddress/PhoneText"></xsl:value-of>
            <span>(*)</span>
          </label>
          <div class="col-md-9 col-xs-12">
            <input type="text" name="Address_Phone" class="form-control">
              <xsl:attribute name="value">
                <xsl:value-of select="/CheckoutAddress/Phone"></xsl:value-of>
              </xsl:attribute>
            </input>
          </div>
        </div>
      </div>
      <div class="form-group">
        <div class="row">
          <label class="col-md-3 col-xs-12">
            <xsl:value-of select="/CheckoutAddress/AddressText"></xsl:value-of>
            <span>(*)</span>
          </label>
          <div class="col-md-9 col-xs-12">
            <input type="text" name="Address_Address" class="form-control">
              <xsl:attribute name="value">
                <xsl:value-of select="/CheckoutAddress/Address"></xsl:value-of>
              </xsl:attribute>
            </input>
          </div>
        </div>
      </div>
      <h3 class="title-form">
        <xsl:text>Thông tin sản phẩm cần báo giá</xsl:text>
      </h3>
      <div class="form-group">
        <div class="row">
          <div class="col-md-9 col-xs-12 col-md-offset-3">
            <input id="autocomplete" type="text" class="form-control">
              <xsl:attribute name="placeholder">
                <xsl:text>Nhập mã sản phẩm</xsl:text>
              </xsl:attribute>
            </input>
          </div>
        </div>
      </div>
      <div class="quotation-products"></div>
      <div class="checkoutBtn">
        <div class="row">
          <div class="col-md-9 col-xs-12 col-md-offset-3 payment_wrap">
            <button type="submit" id="checkout" name="checkout" class="btn btn-payment">
              <xsl:attribute name="onclick">
                <xsl:text>AjaxCheckout.savequotation('</xsl:text>
                <xsl:value-of select="/CheckoutAddress/NextPageUrl"></xsl:value-of>
                <xsl:text>');return false;</xsl:text>
              </xsl:attribute>
              <xsl:text>Gửi</xsl:text>
              <xsl:text> </xsl:text>
              <i class="fa fa-long-arrow-right"></i>
            </button>
          </div>
        </div>
      </div>
    </div>
  </xsl:template>

</xsl:stylesheet>