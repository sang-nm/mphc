<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <div class="order-bg">
      <h3>
        <xsl:text>Thông tin đơn hàng</xsl:text>
      </h3>
      <div class="order-sum-wrap">
        <div class="orderItems">
          <xsl:apply-templates select="/ShoppingCart/CartItem"></xsl:apply-templates>
        </div>
        <div class="itemOrderTotal">
          <xsl:if test="/ShoppingCart/UserPoints>0">
            <div class="row">
              <div class="col-xs-6">
                <span class="order-totalText">Tổng cộng</span>
              </div>
              <div class="col-xs-6 text-right">
                <span class="sub-total">
                  <xsl:value-of select="/ShoppingCart/SubTotal"></xsl:value-of>
                </span>
              </div>
            </div>
            <!--<div class="row">
              <div class="col-xs-6">
                <span class="order-totalText">Điểm tích lũy</span>
              </div>
              <div class="col-xs-6 text-right">
                <span class="point-discount">
                  <xsl:value-of select="/ShoppingCart/PointDiscount"></xsl:value-of>
                </span>
              </div>
            </div>-->
          </xsl:if>
          <div class="row">
            <div class="col-xs-6">
              <span class="order-totalText">Thành tiền</span>
            </div>
            <div class="col-xs-6 text-right">
              <span class="order-total">
                <xsl:value-of select="/ShoppingCart/Total"></xsl:value-of>
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!--<xsl:choose>
      <xsl:when test="/ShoppingCart/LoginUrl!=''">
        <xsl:text>Vui lòng </xsl:text>
        <a>
          <xsl:attribute name="href">
            <xsl:value-of select="/ShoppingCart/LoginUrl"></xsl:value-of>
          </xsl:attribute>
          <xsl:text>Đăng nhập</xsl:text>
        </a>
        <xsl:text> để sử dụng điểm tích lũy</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:if test="/ShoppingCart/UserPoints>0">
          <input id="hdfMaxPoints" type="hidden">
            <xsl:attribute name="value">
              <xsl:value-of select="/ShoppingCart/UserPoints"></xsl:value-of>
            </xsl:attribute>
          </input>
          <input id="hdfCurrentPoint" name="hdfCurrentPoint" type="hidden" value="0"></input>
          <xsl:text>Điểm sử dụng: </xsl:text>
          <span id="currentPoint">0</span>
          <div id="point-slider"></div>
          <span>
            <xsl:text>0</xsl:text>
          </span>
          <span class="pull-right">
            <xsl:value-of select="/ShoppingCart/UserPoints"></xsl:value-of>
          </span>
        </xsl:if>
      </xsl:otherwise>
    </xsl:choose>-->
  </xsl:template>
  <xsl:template match="CartItem">
    <div class="itemOrderCart">
      <div class="row">
        <div class="col-xs-5">
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
            <xsl:value-of select="Title" disable-output-escaping="yes"></xsl:value-of>
          </div>
        </div>
        <div class="col-xs-2">
          <div class="quantity">
            <xsl:text>x</xsl:text>
            <xsl:value-of select="Quantity"></xsl:value-of>
          </div>
        </div>
        <div class="col-xs-5">
          <div class="price">
            <xsl:value-of select="Price"></xsl:value-of>
          </div>
        </div>
      </div>
    </div>
  </xsl:template>
</xsl:stylesheet>