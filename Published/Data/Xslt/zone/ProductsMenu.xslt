<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <xsl:if test="count(/ZoneList/Zone) > 0">
      <nav class="block-productsMenu">
        <ul class="nav">
			<li class="allLink">
				<a>
					<xsl:attribute name="href">
					  <xsl:value-of select="/ZoneList/ParentUrl"></xsl:value-of>
					</xsl:attribute>
					<xsl:attribute name="target">
					  <xsl:value-of select="/ZoneList/Target"></xsl:value-of>
					</xsl:attribute>
					<xsl:value-of select="/ZoneList/AllLinkTxt" disable-output-escaping="yes"></xsl:value-of>
				</a>
			</li>
			<xsl:apply-templates select="/ZoneList/Zone"></xsl:apply-templates>
        </ul>
      </nav>
    </xsl:if>
  </xsl:template>

  <xsl:template match="Zone">
    <li>
      <a>
        <xsl:if test="IsActive='true'">
          <xsl:attribute name="class">
            <xsl:text>active</xsl:text>
          </xsl:attribute>
        </xsl:if>
        <xsl:attribute name="href">
          <xsl:value-of select="Url"></xsl:value-of>
        </xsl:attribute>
        <xsl:attribute name="target">
          <xsl:value-of select="Target"></xsl:value-of>
        </xsl:attribute>
        <xsl:value-of select="Title" disable-output-escaping="yes"></xsl:value-of>
      </a>
    </li>
  </xsl:template>
</xsl:stylesheet>