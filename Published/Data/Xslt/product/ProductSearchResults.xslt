<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <xsl:choose>
      <xsl:when test="count(/ProductList/Product)>0">
        <div class="searchsummary">
          <xsl:value-of select="/ProductList/SearchSumary" disable-output-escaping="yes"></xsl:value-of>
        </div>
        <div class="noli searchresultlist">
          <xsl:apply-templates select="/ProductList/Product"></xsl:apply-templates>
        </div>
      </xsl:when>
      <xsl:otherwise>
        <div class="noresults">
          <xsl:value-of select="/ProductList/NoResults" disable-output-escaping="yes"></xsl:value-of>
        </div>
      </xsl:otherwise>
    </xsl:choose>
    <div class="clearfix"></div>
  </xsl:template>

  <xsl:template match="Product">
    <xsl:if test ="(position() mod 4) = 1">
      <xsl:text disable-output-escaping="yes">&lt;div class='row' &gt;</xsl:text>
    </xsl:if>
    <div class="col-xs-12 col-md-6 col-lg-3 mrb30">
      <div class="item-prd">
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
            <div class="price-prd center-block">
              <xsl:choose>
                <xsl:when test="Price != ''">
                  <xsl:choose>
                    <xsl:when test="OldPrice != ''">
                      <span class="price-old">
                        <xsl:value-of select="OldPrice"></xsl:value-of>
                      </span>
                      <xsl:text> - </xsl:text>
                      <span class="price-new">
                        <xsl:value-of select="Price"></xsl:value-of>
                      </span>
                    </xsl:when>
                    <xsl:otherwise>
                      <span class="price-new">
                        <xsl:value-of select="Price"></xsl:value-of>
                      </span>
                    </xsl:otherwise>
                  </xsl:choose>
                </xsl:when>
                <xsl:otherwise>
                  <span class="price-new">
                    <xsl:value-of select="/ProductList/CallText"></xsl:value-of>
                  </span>
                </xsl:otherwise>
              </xsl:choose>
            </div>
            <a href="#" class="btn-prd">
              <xsl:attribute name="onclick">
                <xsl:text>AjaxCart.addproducttocart_catalog(this);return false;</xsl:text>
              </xsl:attribute>
              <xsl:attribute name="data-productid">
                <xsl:value-of select="ProductId"></xsl:value-of>
              </xsl:attribute>
              <xsl:attribute name="title">
                <xsl:value-of select="Title"></xsl:value-of>
              </xsl:attribute>
              <span>
                <xsl:text>Mua ngay</xsl:text>
              </span>
            </a>
          </figcaption>
        </figure>
      </div>
    </div>
    <xsl:if test ="(position() mod 4) = 0">
      <xsl:text disable-output-escaping="yes">&lt;/div&gt;</xsl:text>
    </xsl:if>
    <xsl:if test ="position()=last()">
      <xsl:if test ="last() mod 4 != 0">
        <xsl:text disable-output-escaping="yes">&lt;/div&gt;</xsl:text>
      </xsl:if>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>