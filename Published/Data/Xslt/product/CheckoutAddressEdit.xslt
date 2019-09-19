<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <h1 class="title-page">
      <xsl:text>Bước 2: Địa chỉ giao hàng</xsl:text>
    </h1>
    <div class="checkout-address form-join-wrap form-horizontal">
      <div class="form-group">
        <div class="row">
          <label class="col-sm-3 col-xs-12">
            <xsl:value-of select="/CheckoutAddress/FullNameText"></xsl:value-of>
            <span>(*)</span>
          </label>
          <div class="col-sm-9 col-xs-12">
            <input type="text" name="Address_FirstName" class="form-control">
              <xsl:attribute name="value">
                <xsl:value-of select="/CheckoutAddress/FirstName"></xsl:value-of>
              </xsl:attribute>
            </input>
          </div>
        </div>
      </div>
      <!--<div class="form-group">
        <div class="row">
          <label class="col-sm-3 col-xs-12">
            <xsl:value-of select="/CheckoutAddress/LastNameText"></xsl:value-of>
            <span>(*)</span>
          </label>
          <div class="col-sm-9 col-xs-12">
            <input type="text" name="Address_LastName" class="form-control">
              <xsl:attribute name="value">
                <xsl:value-of select="/CheckoutAddress/LastName"></xsl:value-of>
              </xsl:attribute>
            </input>
          </div>
        </div>
      </div>-->
      <div class="form-group">
        <div class="row">
          <label class="col-sm-3 col-xs-12">
            <xsl:value-of select="/CheckoutAddress/EmailText"></xsl:value-of>
          </label>
          <div class="col-sm-9 col-xs-12">
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
          <label class="col-sm-3 col-xs-12">
            <xsl:value-of select="/CheckoutAddress/PhoneText"></xsl:value-of>
            <span>(*)</span>
          </label>
          <div class="col-sm-9 col-xs-12">
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
          <label class="col-sm-3 col-xs-12">
            <xsl:value-of select="/CheckoutAddress/AddressText"></xsl:value-of>
            <span>(*)</span>
          </label>
          <div class="col-sm-9 col-xs-12">
            <input type="text" name="Address_Address" class="form-control">
              <xsl:attribute name="value">
                <xsl:value-of select="/CheckoutAddress/Address"></xsl:value-of>
              </xsl:attribute>
            </input>
          </div>
        </div>
      </div>
      <div class="form-group">
        <div class="row">
          <label class="col-sm-3 col-xs-12">
            <xsl:value-of select="/CheckoutAddress/ProvinceText"></xsl:value-of>
            <span>(*)</span>
          </label>
          <div class="col-sm-9 col-xs-12">
            <select name="Address_Province" onchange="AjaxCheckout.getdistrictsbyprovinceguid(this)">
              <option value="">
                <xsl:value-of select="/CheckoutAddress/SelectProvinceText"></xsl:value-of>
              </option>
              <xsl:apply-templates select="/CheckoutAddress/Provinces"></xsl:apply-templates>
            </select>
          </div>
        </div>
      </div>
      <div class="form-group">
        <div class="row">
          <label class="col-sm-3 col-xs-12">
            <xsl:value-of select="/CheckoutAddress/DistrictText"></xsl:value-of>
            <span>(*)</span>
          </label>
          <div class="col-sm-9 col-xs-12">
            <select name="Address_District">
              <option value="">
                <xsl:value-of select="/CheckoutAddress/SelectDistrictText"></xsl:value-of>
              </option>
              <xsl:apply-templates select="/CheckoutAddress/Districts"></xsl:apply-templates>
            </select>
          </div>
        </div>
      </div>
      <div class="form-group">
        <div class="row">
          <label class="col-sm-3 col-xs-12">
            <xsl:text>Ghi chú</xsl:text>
          </label>
          <div class="col-sm-9 col-xs-12">
            <textarea name="OrderNote" class="form-control note">
              <xsl:value-of select="/CheckoutAddress/OrderNote"></xsl:value-of>
            </textarea>
          </div>
        </div>
      </div>
      <!--<div class="form-group ct-button">
        <div class="frm-btnwrap">
          <div class="frm-btn">
            <input type="submit" class="ct-button btn btn-default" value="Đặt mua">
              <xsl:attribute name="onclick">
                <xsl:text>AjaxCheckout.saveorder(true, '</xsl:text>
                <xsl:value-of select="/CheckoutAddress/NextPageUrl"></xsl:value-of>
                <xsl:text>');return false;</xsl:text>
              </xsl:attribute>
            </input>
          </div>
        </div>
      </div>-->
    </div>
  </xsl:template>

  <xsl:template match="Provinces">
    <option>
      <xsl:if test="IsActive='true'">
        <xsl:attribute name="selected">
          <xsl:text>selected</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <xsl:attribute name="value">
        <xsl:value-of select="Guid"></xsl:value-of>
      </xsl:attribute>
      <xsl:value-of select="Title"></xsl:value-of>
    </option>
  </xsl:template>
  <xsl:template match="Districts">
    <option>
      <xsl:if test="IsActive='true'">
        <xsl:attribute name="selected">
          <xsl:text>selected</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <xsl:attribute name="value">
        <xsl:value-of select="Guid"></xsl:value-of>
      </xsl:attribute>
      <xsl:value-of select="Title"></xsl:value-of>
    </option>
  </xsl:template>
</xsl:stylesheet>