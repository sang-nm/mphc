<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <div class="block-prdSpecial prdSpecial-owl">
      <!--<div class="tabs">
        <ul>
          <li>
            <a href="javascript:void(0)" class="btn-spbc active">
              <span>Sản phẩm bán chạy</span>
            </a>
          </li>
          <li>
            <a href="javascript:void(0)" class="btn-spkm">
              <span>Sản phẩm khuyến mãi</span>
            </a>
          </li>
        </ul>
      </div>-->
      <h3>
        <xsl:value-of select="/ProductList/ModuleTitle" disable-output-escaping="yes"></xsl:value-of>
      </h3>
      <div class="slider">
        <div class="owl-carousel">
          <xsl:apply-templates select="/ProductList/Product"></xsl:apply-templates>
        </div>
      </div>
    </div>
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
        <xsl:if test="floor(ShowOption div 1) mod 2 = 1">
          <div class="promotion op1">
            <span>Hot</span>
          </div>
        </xsl:if>
        <xsl:if test="floor(ShowOption div 2) mod 2 = 1">
          <div class="promotion op2">
            <span>Sale</span>
          </div>
        </xsl:if>
        <xsl:if test="floor(ShowOption div 4) mod 2 = 1">
          <div class="promotion op3">
            <span>New</span>
          </div>
        </xsl:if>
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
        <xsl:if test="floor(ShowOption div 1) mod 2 = 1">
          <div class="promotion op1">
            <span>Hot</span>
          </div>
        </xsl:if>
        <xsl:if test="floor(ShowOption div 2) mod 2 = 1">
          <div class="promotion op2">
            <span>Sale</span>
          </div>
        </xsl:if>
        <xsl:if test="floor(ShowOption div 4) mod 2 = 1">
          <div class="promotion op3">
            <span>New</span>
          </div>
        </xsl:if>
      </figure>
    </div>
  </xsl:template>
</xsl:stylesheet>