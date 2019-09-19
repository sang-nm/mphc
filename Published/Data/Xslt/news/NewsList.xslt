<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <div class="block-newsList">
      <h1 class="title-page">
        <xsl:value-of select="/NewsList/ZoneTitle"></xsl:value-of>
      </h1>
      <div class="row">
        <xsl:apply-templates select="/NewsList/News"></xsl:apply-templates>
      </div>
    </div>
  </xsl:template>

  <xsl:template match="News">
    <div class="col-xs-12 mrb30">
      <div class="item-news">
        <div class="row">
          <div class="col-xs-12 col-md-7 col-lg-6">
            <time>
              <span class="date">
                <xsl:value-of select="CreatedDD"></xsl:value-of>
              </span>
              <span class="month">
                <xsl:text>Th. </xsl:text>
                <xsl:value-of select="CreatedMM"></xsl:value-of>
              </span>
            </time>
            <div class="thumb">
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
                <img class="hvr-grow">
                  <xsl:attribute name="src">
                    <xsl:value-of select="ImageUrl"></xsl:value-of>
                  </xsl:attribute>
                  <xsl:attribute name="alt">
                    <xsl:value-of select="Title"></xsl:value-of>
                  </xsl:attribute>
                </img>
              </a>
            </div>
          </div>
          <div class="col-xs-12 col-md-5 col-lg-6">
            <div class="news-content">
              <h3>
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
                  <xsl:value-of select="Title"></xsl:value-of>
                </a>
                <xsl:value-of select="EditLink" disable-output-escaping="yes"></xsl:value-of>
              </h3>
              <div class="brief">
                <xsl:value-of select="BriefContent" disable-output-escaping="yes"></xsl:value-of>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </xsl:template>
</xsl:stylesheet>