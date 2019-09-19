<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <div class="block-newsDetail">
      <h1 class="title-page">
        <xsl:value-of select="/NewsDetail/Title"></xsl:value-of>
        <xsl:value-of select="/NewsDetail/EditLink" disable-output-escaping="yes"></xsl:value-of>
      </h1>
      <div class="fullContent">
        <xsl:value-of select="/NewsDetail/FullContent" disable-output-escaping="yes"></xsl:value-of>
      </div>
      <div class="Tags">
        <xsl:apply-templates select="/NewsDetail/NewsTags/NewsTag"></xsl:apply-templates>
      </div>
      <div class="social clearfix">
        <div class="fb-send"></div>
        <div class="face-like">
          <xsl:value-of select="/NewsDetail/FacebookLike" disable-output-escaping="yes"></xsl:value-of>
        </div>
        <div class="fb-share-button" data-layout="button_count"></div>
        <div class="google">
          <xsl:value-of select="/NewsDetail/PlusOne" disable-output-escaping="yes"></xsl:value-of>
        </div>
        <div class="tweet">
          <xsl:value-of select="/NewsDetail/TweetThis" disable-output-escaping="yes"></xsl:value-of>
        </div>
      </div>
      <xsl:if test="count(/NewsDetail/NewsOther)>0">
        <div class="block-newsOther">
          <h3>
            <xsl:value-of select="/NewsDetail/NewsOtherText"></xsl:value-of>
          </h3>
          <div class="row">
            <xsl:apply-templates select="/NewsDetail/NewsOther"></xsl:apply-templates>
          </div>
        </div>
      </xsl:if>
    </div>
  </xsl:template>
  <xsl:template match="NewsTag">
    <a href="#">
      <xsl:attribute name="href">
        <xsl:text>/SearchResults.aspx?q=</xsl:text>
        <xsl:value-of select="Tag"></xsl:value-of>
      </xsl:attribute>
      <xsl:attribute name="target">
        <xsl:text>_Target</xsl:text>
      </xsl:attribute>
      <xsl:value-of select="Tag"></xsl:value-of>
    </a>
  </xsl:template>
  <xsl:template match="NewsOther">
    <div class="col-xs-12">
      <div class="item-newsOther">
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
          <xsl:value-of select="Title" disable-output-escaping="yes"></xsl:value-of>
        </a>
      </div>
    </div>
  </xsl:template>
</xsl:stylesheet>