<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <xsl:if test="count(/ZoneList/Zone) > 0">
      <xsl:apply-templates select="/ZoneList/Zone"></xsl:apply-templates>
    </xsl:if>
  </xsl:template>

  <xsl:template match="Zone">
    <xsl:if test="count(Product) > 0">
      <div class="block-zoneProductsList prdSpecial-owl">
        <h3>
          <xsl:value-of select="Title"></xsl:value-of>
        </h3>
        <div class="slider">
          <div class="owl-carousel">
            <xsl:apply-templates select="Product"></xsl:apply-templates>
          </div>
        </div>
      </div>
    </xsl:if>
  </xsl:template>

  <xsl:template match="Product">
    <div class="item-prd clearfix">
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
          <a class="btn-prd">
			<xsl:attribute name="href">
                <xsl:value-of select="Url"></xsl:value-of>
			</xsl:attribute>
			<xsl:attribute name="title">
				<xsl:value-of select="Title"></xsl:value-of>
			</xsl:attribute>
            <span>
              <i class="fa fa-shopping-cart" aria-hidden="true"></i>
            </span>
          </a>
        </figcaption>
      </figure>
      <figure class="prd-hover">
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
              <xsl:value-of select="ThumbnailUrl"></xsl:value-of>
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
          <div class="brief">
            <xsl:value-of select="BriefContent" disable-output-escaping="yes"></xsl:value-of>
          </div>
          <a class="btn-prd">
            <xsl:attribute name="href">
                <xsl:value-of select="Url"></xsl:value-of>
			</xsl:attribute>
			<xsl:attribute name="title">
				<xsl:value-of select="Title"></xsl:value-of>
			</xsl:attribute>
            <span>
              <i class="fa fa-shopping-cart" aria-hidden="true"></i>
            </span>
          </a>
        </figcaption>
      </figure>
    </div>
  </xsl:template>
</xsl:stylesheet>