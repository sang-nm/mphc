<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <section class="check-out clearfix">
      <xsl:choose>
        <xsl:when test="count(/ShoppingCart/CartItem)=0">
          <h1 class="check-title">
            <i class="fa fa-cart-arrow-down"></i>
            <xsl:value-of select="/ShoppingCart/CartEmptyText"></xsl:value-of>
          </h1>
        </xsl:when>
        <xsl:otherwise>
          <h1 class="title-page">
            <xsl:text>Bước 1: Giỏ hàng của bạn</xsl:text>
          </h1>
          <div class="table-responsive">
            <table class="table table-hover">
              <thead>
                <tr>
                  <th>
                    <xsl:value-of select="/ShoppingCart/ProductText"></xsl:value-of>
                  </th>
                  <th>
                    <xsl:value-of select="/ShoppingCart/PriceText"></xsl:value-of>
                  </th>
                  <th>
                    <xsl:value-of select="/ShoppingCart/QuantityText"></xsl:value-of>
                  </th>
                  <th>
                    <xsl:value-of select="/ShoppingCart/ItemTotalText"></xsl:value-of>
                  </th>
                  <th>
                    <xsl:value-of select="/ShoppingCart/RemoveText"></xsl:value-of>
                  </th>
                </tr>
              </thead>
              <tbody>
                <xsl:apply-templates select="/ShoppingCart/CartItem"></xsl:apply-templates>
              </tbody>
            </table>
          </div>
          <div class="discount_payment_wrap clearfix">
            <div class="row">
              <div class="col-xs-12 col-md-4">
                <a class="checkout-back">
                  <xsl:attribute name="href">
                    <xsl:text>/</xsl:text>
                  </xsl:attribute>
                  <xsl:attribute name="target">
                    <xsl:value-of select="Target"></xsl:value-of>
                  </xsl:attribute>
                  <i class="fa fa-long-arrow-left"></i>
                  <xsl:text> Tiếp tục mua hàng</xsl:text>
                </a>
              </div>
              <div class="col-xs-12 col-md-8">
                <div class="payment_wrap">
                  <div class="paymentTotal">
                    <div class="item-pm">
                      <span>
                        <xsl:value-of select="/ShoppingCart/TotalText"></xsl:value-of>
                        <xsl:text>: </xsl:text>
                      </span>
                      <span class="total-number">
                        <xsl:value-of select="/ShoppingCart/Total"></xsl:value-of>
                      </span>
                    </div>
                  </div>
                  <button type="submit" id="checkout" name="checkout" class="btn btn-payment">
                    <xsl:attribute name="onclick">
                      <xsl:text>setLocation('</xsl:text>
                      <xsl:value-of select="/ShoppingCart/CheckoutUrl"></xsl:value-of>
                      <xsl:text>');return false;</xsl:text>
                    </xsl:attribute>
                    <xsl:value-of select="/ShoppingCart/CheckoutProcessText"></xsl:value-of>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </xsl:otherwise>
      </xsl:choose>
    </section>
  </xsl:template>
  <xsl:template match="CartItem">
    <tr>
      <td>
        <div class="thumb">
          <a>
            <xsl:attribute name="href">
              <xsl:value-of select="Url"></xsl:value-of>
            </xsl:attribute>
            <xsl:attribute name="target">
              <xsl:value-of select="Target"></xsl:value-of>
            </xsl:attribute>
            <img>
              <xsl:attribute name="src">
                <xsl:value-of select="ThumbnailUrl"></xsl:value-of>
              </xsl:attribute>
              <xsl:attribute name="alt">
                <xsl:value-of select="Title"></xsl:value-of>
              </xsl:attribute>
            </img>
          </a>
        </div>
        <div class="prd-name">
          <a class="product-name">
            <xsl:attribute name="href">
              <xsl:value-of select="Url"></xsl:value-of>
            </xsl:attribute>
            <xsl:attribute name="target">
              <xsl:value-of select="Target"></xsl:value-of>
            </xsl:attribute>
            <span>
              <xsl:value-of select="Title" disable-output-escaping="yes"></xsl:value-of>
            </span>
          </a>
        </div>
      </td>
      <td>
        <xsl:value-of select="Price"></xsl:value-of>
      </td>
      <td>
        <xsl:choose>
          <xsl:when test="count(Quantities)>0">
            <select>
              <xsl:attribute name="name">
                <xsl:text>itemquantity</xsl:text>
                <xsl:value-of select="ItemGuid"></xsl:value-of>
              </xsl:attribute>
              <xsl:apply-templates select="Quantities"></xsl:apply-templates>
            </select>
          </xsl:when>
          <xsl:otherwise>
            <input type="text" maxlength="5" class="qty-input" onchange="AjaxCart.updatecart();return false;">
              <xsl:attribute name="name">
                <xsl:text>itemquantity</xsl:text>
                <xsl:value-of select="ItemGuid"></xsl:value-of>
              </xsl:attribute>
              <xsl:attribute name="value">
                <xsl:value-of select="Quantity"></xsl:value-of>
              </xsl:attribute>
            </input>
          </xsl:otherwise>
        </xsl:choose>
      </td>
      <td class="total-price">
        <xsl:value-of select="ItemTotal"></xsl:value-of>
      </td>
      <td>
        <a href="#" onclick="AjaxCart.removefromcart(this);return false;">
          <xsl:attribute name="data-itemguid">
            <xsl:value-of select="ItemGuid"></xsl:value-of>
          </xsl:attribute>
          <i class="fa fa-trash-o"></i>
        </a>
      </td>
    </tr>
  </xsl:template>
  <xsl:template match="Quantities">
    <option>
      <xsl:attribute name="value">
        <xsl:value-of select="Quantity"></xsl:value-of>
      </xsl:attribute>
      <xsl:value-of select="Quantity"></xsl:value-of>
    </option>
  </xsl:template>
</xsl:stylesheet>