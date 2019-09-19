<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <div class="block-productsList">
      <h1 class="title-page">
        <xsl:value-of select="/ProductList/ZoneTitle"></xsl:value-of>
      </h1>
      <div class="row">
        <xsl:apply-templates select="/ProductList/Product"></xsl:apply-templates>
      </div>
    </div>
  </xsl:template>
  <xsl:template match="Product">
    <div class="col-xs-6 col-md-6 col-lg-3 mrb30">
      <div class="item-prd">
        <figure>
          <a class="center-block">
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
          <figcaption>
            <h3>
              <a>
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
              <xsl:value-of select="EditLink" disable-output-escaping="yes"></xsl:value-of>
            </h3>
            <div class="price">
              <xsl:value-of select="Price" disable-output-escaping="yes"></xsl:value-of>
            </div>
            <a href="#" class="btn-prd">
              <xsl:attribute name="onclick">
                <xsl:text>AjaxCart.addproducttocart_catalog(this);return false;</xsl:text>
              </xsl:attribute>
              <xsl:attribute name="data-productid">
                <xsl:value-of select="ProductId"></xsl:value-of>
              </xsl:attribute>
              <xsl:attribute name="title">
                <xsl:value-of select="Title"></xsl:value-of>
              </xsl:attribute>
              <i class="fa fa-shopping-cart" aria-hidden="true"></i>
              <xsl:text> </xsl:text>
              <xsl:value-of select="/ProductList/OrderBuyTxt" disable-output-escaping="yes"></xsl:value-of>
            </a>
          </figcaption>
        </figure>
      </div>
    </div>
  </xsl:template>
</xsl:stylesheet>