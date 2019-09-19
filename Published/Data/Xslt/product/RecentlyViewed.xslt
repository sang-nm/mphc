<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <xsl:if test="count(/ProductList/Product)>0">
      <div class="wrap_prd_recently">
        <h2>
          <i class="fa fa-eye"></i>
          Sản phẩm đã xem
        </h2>
        <span class="btn_viewed btn_top"></span>
        <ul class="prd_recently">
          <xsl:apply-templates select="/ProductList/Product"></xsl:apply-templates>
        </ul>
        <span class="btn_viewed btn_bottom"></span>
      </div>
    </xsl:if>
  </xsl:template>
  <xsl:template match="Product">
    <li>
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
        <xsl:choose>
          <xsl:when test="ThumbnailUrl != ''">
            <img width="50">
              <xsl:attribute name="src">
                <xsl:value-of select="ThumbnailUrl"></xsl:value-of>
              </xsl:attribute>
              <xsl:attribute name="alt">
                <xsl:value-of select="Title"></xsl:value-of>
              </xsl:attribute>
            </img>
          </xsl:when>
          <xsl:when test="ImageUrl != ''">
            <img width="50">
              <xsl:attribute name="src">
                <xsl:value-of select="ImageUrl"></xsl:value-of>
              </xsl:attribute>
              <xsl:attribute name="alt">
                <xsl:value-of select="Title"></xsl:value-of>
              </xsl:attribute>
            </img>
          </xsl:when>
        </xsl:choose>
      </a>

      <div class="clear"></div>
    </li>
  </xsl:template>
</xsl:stylesheet>