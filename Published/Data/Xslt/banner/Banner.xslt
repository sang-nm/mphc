<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <xsl:if test="count(/BannerList/Banner) > 0">
      <div class="bannerPage">
        <xsl:apply-templates select="/BannerList/Banner" mode="image"></xsl:apply-templates>
      </div>
    </xsl:if>
  </xsl:template>

  <xsl:template match="Banner" mode="image">
    <figure>
      <img>
        <xsl:attribute name="src">
          <xsl:value-of select="ImageUrl"></xsl:value-of>
        </xsl:attribute>
        <xsl:attribute name="alt">
          <xsl:value-of select="Title" disable-output-escaping="yes"></xsl:value-of>
        </xsl:attribute>
      </img>
      <figcaption>
        <div class="container">
          <div class="caption">
            <xsl:value-of select="Description" disable-output-escaping="yes"></xsl:value-of>
          </div>
        </div>
      </figcaption>
    </figure>
  </xsl:template>
</xsl:stylesheet>