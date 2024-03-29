<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <ul class="wrap-suggestion">
      <xsl:apply-templates select="/ProductList/Product"></xsl:apply-templates>
    </ul>
  </xsl:template>

  <xsl:template match="Product">
    <xsl:if test="position()&lt;6">
      <li>
        <a class="clearfix">
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
              <div class="col-xs-3 col-md-2">
                <img>
                  <xsl:attribute name="src">
                    <xsl:value-of select="ThumbnailUrl"></xsl:value-of>
                  </xsl:attribute>
                </img>
              </div>
            </xsl:when>
            <xsl:when test="ImageUrl != ''">
              <div class="col-xs-3 col-md-2">
                <img>
                  <xsl:attribute name="src">
                    <xsl:value-of select="ThumbnailUrl"></xsl:value-of>
                  </xsl:attribute>
                </img>
              </div>
            </xsl:when>
          </xsl:choose>
          <div class="col-xs-9 col-md-10">
            <div class="prd-info">
              <h4>
                <xsl:value-of select="Code"></xsl:value-of>
              </h4>
              <h3>
                <xsl:value-of select="Title"></xsl:value-of>
              </h3>
              <span class="price">
                <xsl:value-of select="Price"></xsl:value-of>
              </span>
            </div>
          </div>          
        </a>
      </li>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>