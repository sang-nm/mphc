<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <div class="product-speical">
      <h3>
        <xsl:value-of select="/ProductList/ModuleTitle"></xsl:value-of>
      </h3>
      <xsl:apply-templates select="/ProductList/Product"></xsl:apply-templates>
    </div>
  </xsl:template>
  <xsl:template match="Product">
    <div class="row mrb15">
      <div class="col-xs-3">
        <div class="thumb">
          <xsl:if test="ImageUrl != ''">
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
          </xsl:if>
        </div>
      </div>
      <div class="col-xs-9">
        <div class="content">
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
            <span class="autoCutStr_30">
              <xsl:value-of select="Title"></xsl:value-of>
            </span>
          </a>
          <xsl:value-of select="EditLink" disable-output-escaping="yes"></xsl:value-of>
          <p class="price">
            <xsl:value-of select="Price"></xsl:value-of>
          </p>
        </div>
      </div>
    </div>
  </xsl:template>
</xsl:stylesheet>