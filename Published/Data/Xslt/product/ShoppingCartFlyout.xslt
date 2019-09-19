<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <span class="mobile_prd_menu hidden-lg"></span>
    <div class="cart-box awemenu-icon menu-shopping-cart">
      <i class="fa fa-shopping-cart" aria-hidden="true"></i>
      <xsl:choose>
        <xsl:when test="count(/ShoppingCart/CartItem)>0">
          <span>
            <xsl:value-of select="/ShoppingCart/TotalProducts"></xsl:value-of>
            <xsl:text> sản phẩm</xsl:text>
          </span>
        </xsl:when>
        <xsl:otherwise>
          <span>
            <xsl:text>Giỏ hàng</xsl:text>
          </span>
        </xsl:otherwise>
      </xsl:choose>
    </div>
    <div id="flyout-cart" class="flyout-cart">
      <div class="mini-shopping-cart">
        <div class="count">
          <xsl:value-of select="/ShoppingCart/CartSummaryText" disable-output-escaping="yes"></xsl:value-of>
        </div>
        <xsl:if test="count(/ShoppingCart/CartItem)>0">
          <div class="items">
            <xsl:apply-templates select="/ShoppingCart/CartItem"></xsl:apply-templates>
          </div>
          <div class="totals">
            <xsl:value-of select="/ShoppingCart/SubTotalText"></xsl:value-of>:
            <strong>
              <xsl:value-of select="/ShoppingCart/SubTotal"></xsl:value-of>
            </strong>
          </div>
          <div class="buttons">
            <a>
              <xsl:attribute name="href">
                <xsl:value-of select="/ShoppingCart/CartPageUrl"></xsl:value-of>
              </xsl:attribute>
              <xsl:value-of select="/ShoppingCart/CartText"></xsl:value-of>
            </a>
          </div>
        </xsl:if>
      </div>
    </div>

    <div class="productAddedToCartWindow hidden">
      <div class="productAddedToCartWindowTitle">
        <i class="fa fa-shopping-cart" aria-hidden="true"></i>
        <xsl:text> </xsl:text>
        Sản phẩm thêm vào giỏ hàng
      </div>
      <div class="productAddedToCartItem">
        <xsl:apply-templates select="/ShoppingCart/CartItem" mode="LastAddedItem"></xsl:apply-templates>
      </div>
      <div class="productAddedToCartWindowSummary">
        <a class="continueShoppingLink" href="" onclick="closePopupNotification(); return false;">
          <i class="fa fa-long-arrow-left" aria-hidden="true"></i>
          <xsl:text> </xsl:text>
          <xsl:value-of select="/ShoppingCart/ContinueShoppingText" disable-output-escaping="yes"></xsl:value-of>
        </a>
        <div class="nextLink">
          <button type="submit" class="button-1 productAddedToCartWindowCheckout" onclick="setLocation('/cart');">
            <xsl:attribute name="onclick">
              <xsl:text>setLocation('</xsl:text>
              <xsl:value-of select="/ShoppingCart/CartPageUrl"></xsl:value-of>
              <xsl:text>');return false;</xsl:text>
            </xsl:attribute>
            <xsl:value-of select="/ShoppingCart/CartText"></xsl:value-of>
            <xsl:text> </xsl:text>
            <i class="fa fa-long-arrow-right" aria-hidden="true"></i>
          </button>
        </div>
      </div>
    </div>
  </xsl:template>

  <xsl:template match="CartItem" mode="LastAddedItem">
    <xsl:if test="LastAddedItem='true'">
      <div class="table-responsive">
        <table class="table table-hover">
          <thead>
            <tr>
              <th>
                <xsl:text>Sản phẩm</xsl:text>
              </th>
              <th>
                <xsl:text>Số lượng</xsl:text>
              </th>
              <th>
                <xsl:text>Đơn giá</xsl:text>
              </th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>
                <div class="productAddedToCartWindowImage">
                  <a>
                    <xsl:attribute name="href">
                      <xsl:value-of select="Url"></xsl:value-of>
                    </xsl:attribute>
                    <xsl:attribute name="target">
                      <xsl:value-of select="Target"></xsl:value-of>
                    </xsl:attribute>
                    <xsl:attribute name="title">
                      <xsl:value-of select="Title"></xsl:value-of>
                    </xsl:attribute>
                    <img>
                      <xsl:attribute name="src">
                        <xsl:value-of select="ImageUrl"></xsl:value-of>
                      </xsl:attribute>
                      <xsl:attribute name="alt">
                        <xsl:value-of select="Title"></xsl:value-of>
                      </xsl:attribute>
                    </img>
                  </a>
                </div>
                <div class="productAddedToCartWindowDescription">
                  <h3>
                    <a>
                      <xsl:attribute name="href">
                        <xsl:value-of select="Url"></xsl:value-of>
                      </xsl:attribute>
                      <xsl:attribute name="target">
                        <xsl:value-of select="Target"></xsl:value-of>
                      </xsl:attribute>
                      <xsl:value-of select="Title"></xsl:value-of>
                    </a>
                  </h3>
                  <xsl:apply-templates select="Attributes"></xsl:apply-templates>
                </div>
              </td>
              <td>                
                <span class="quantity">
                  <xsl:value-of select="Quantity"></xsl:value-of>
                </span>
              </td>
              <td>
                <strong class="price">
                  <xsl:value-of select="Price"></xsl:value-of>
                </strong>                
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </xsl:if>
  </xsl:template>

  <xsl:template match="CartItem">
    <div class="item">
      <div class="picture">
        <xsl:if test="ThumbnailUrl != ''">
          <a>
            <xsl:attribute name="href">
              <xsl:value-of select="Url"></xsl:value-of>
            </xsl:attribute>
            <xsl:attribute name="target">
              <xsl:value-of select="Target"></xsl:value-of>
            </xsl:attribute>
            <xsl:attribute name="title">
              <xsl:value-of select="Title"></xsl:value-of>
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
        </xsl:if>
      </div>
      <div class="product">
        <div class="name">
          <a>
            <xsl:attribute name="href">
              <xsl:value-of select="Url"></xsl:value-of>
            </xsl:attribute>
            <xsl:attribute name="target">
              <xsl:value-of select="Target"></xsl:value-of>
            </xsl:attribute>
            <xsl:value-of select="Title"></xsl:value-of>
          </a>
          <xsl:apply-templates select="Attributes"></xsl:apply-templates>
        </div>
        <div class="price">
          <xsl:value-of select="/ShoppingCart/PriceText"></xsl:value-of>:
          <span>
            <xsl:value-of select="Price"></xsl:value-of>
          </span>
        </div>
        <div class="quantity">
          <xsl:value-of select="/ShoppingCart/QuantityText"></xsl:value-of>:
          <span>
            <xsl:value-of select="Quantity"></xsl:value-of>
          </span>
        </div>
      </div>
    </div>
  </xsl:template>

  <xsl:template match="Attributes">
    <div class="option">
      <span class="title">
        <xsl:value-of select="Title"></xsl:value-of>
        <xsl:text>: </xsl:text>
      </span>
      <span class="value">
        <xsl:apply-templates select="Options"></xsl:apply-templates>
      </span>
    </div>
  </xsl:template>
  <xsl:template match="Options">
    <xsl:if test="IsActive='true'">
      <xsl:value-of select="Title"></xsl:value-of>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>