<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>
  <xsl:template match="/">
    <div class="block-showroom">
      <h1 class="title-page">
        <xsl:value-of select="/DealerList/ModuleTitle" disable-output-escaping="yes"></xsl:value-of>
      </h1>
      <xsl:apply-templates select="/DealerList/Dealer" mode="search"></xsl:apply-templates>
    </div>
  </xsl:template>
  <xsl:template match="Dealer" mode="search">
    <div class="dealer-item">
      <div class="row no-gutters">
        <div class="col-xs-12 col-md-12 col-lg-6">
          <div class="thumb">
            <img>
              <xsl:attribute name="src">
                <xsl:value-of select="ImageUrl"></xsl:value-of>
              </xsl:attribute>
              <xsl:attribute name="alt">
                <xsl:value-of select="Title"></xsl:value-of>
              </xsl:attribute>
            </img>
          </div>
        </div>
        <div class="col-xs-12 col-md-12 col-lg-6">
			<div class="dealerContentWrap">
			  <div class="dealer-content">
				<h3>
				  <xsl:value-of select="Title" disable-output-escaping="yes"></xsl:value-of>
				  <xsl:value-of select="EditLink" disable-output-escaping="yes"></xsl:value-of>
				</h3>
				<div class="address">
				  <p>
					<xsl:value-of select="Address" disable-output-escaping="yes"></xsl:value-of>
				  </p>
				  <p>
					<xsl:if test="Phone != ''">
					  <xsl:value-of select="/DealerList/PhoneText" disable-output-escaping="yes"></xsl:value-of>: <xsl:value-of select="Phone" disable-output-escaping="yes"></xsl:value-of>
					</xsl:if>
				  </p>
				  <p>
					<xsl:if test="Email != ''">
					  <xsl:value-of select="/DealerList/EmailText" disable-output-escaping="yes"></xsl:value-of>: <xsl:value-of select="Email" disable-output-escaping="yes"></xsl:value-of>
					</xsl:if>
				  </p>
				  <xsl:if test="Map != ''">
					<a class="amaps fancybox-iframe">
					  <xsl:attribute name="href">
						<xsl:value-of select="Map"></xsl:value-of>
					  </xsl:attribute>
					  <i class="fa fa-map-marker"></i>
					  <xsl:text> </xsl:text>
					  <xsl:value-of select="/DealerList/ViewMapText" disable-output-escaping="yes"></xsl:value-of>
					</a>
				  </xsl:if>
				</div>
			  </div>
			</div>
        </div>
      </div>
    </div>
    <!--<xsl:value-of select="Website" disable-output-escaping="yes"></xsl:value-of>
            <xsl:value-of select="ImageUrl"></xsl:value-of>
            <xsl:value-of select="Map"></xsl:value-of>-->
  </xsl:template>
</xsl:stylesheet>