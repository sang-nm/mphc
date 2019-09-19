<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <div class="block-newsSpecial">
      <div class="news-special">
        <div class="slider">
          <div class="owl-carousel">
            <xsl:apply-templates select="/NewsList/News"></xsl:apply-templates>
          </div>
        </div>
      </div>
    </div>
  </xsl:template>

  <xsl:template match="News">
    <div class="item-nsc">
      <h3>
        <a>
          <xsl:attribute name="target">
            <xsl:value-of select="Target"></xsl:value-of>
          </xsl:attribute>
          <xsl:attribute name="href">
            <xsl:value-of select="Url"></xsl:value-of>
          </xsl:attribute>
          <xsl:attribute name="title">
            <xsl:value-of select="Title"></xsl:value-of>
          </xsl:attribute>
          <xsl:value-of select="Title" disable-output-escaping="yes"></xsl:value-of>
        </a>
      </h3>
      <div class="desc">
		<div class="autoCutStr_130">
			<xsl:value-of select="BriefContent" disable-output-escaping="yes"></xsl:value-of>
		</div>
      </div>
    </div>
  </xsl:template>
</xsl:stylesheet>